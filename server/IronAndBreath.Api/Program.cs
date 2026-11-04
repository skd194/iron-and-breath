using System.Text;
using IronAndBreath.Api.Ai;
using IronAndBreath.Api.Auth;
using IronAndBreath.Api.Services;
using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure;
using IronAndBreath.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "ClientApp";

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Let Swagger UI send a Bearer token.
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the JWT from /api/auth/login (no 'Bearer ' prefix).",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IVideoResolver, VideoResolver>();
builder.Services.Configure<WarmUpOptions>(builder.Configuration.GetSection(WarmUpOptions.SectionName));

// ---- AI coach ----
builder.Services.Configure<AiOptions>(builder.Configuration.GetSection(AiOptions.SectionName));
// Allow the key to come from the CLAUDE_API_KEY environment variable (never hardcoded).
builder.Services.PostConfigure<AiOptions>(o =>
{
    if (string.IsNullOrWhiteSpace(o.ApiKey))
    {
        o.ApiKey = Environment.GetEnvironmentVariable("CLAUDE_API_KEY") ?? string.Empty;
    }
});
var aiOptions = builder.Configuration.GetSection(AiOptions.SectionName).Get<AiOptions>() ?? new AiOptions();
var aiKeyPresent = !string.IsNullOrWhiteSpace(aiOptions.ApiKey)
                   || !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CLAUDE_API_KEY"));
if (aiKeyPresent)
{
    builder.Services.AddHttpClient<IAiProvider, ClaudeAiProvider>(c => c.Timeout = TimeSpan.FromMinutes(5));
}
else
{
    // No key: use the offline stub so the coach still works in dev.
    builder.Services.AddScoped<IAiProvider, StubAiProvider>();
}

// ---- Authentication / authorization ----
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection(GoogleAuthOptions.SectionName));

var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
if (Encoding.UTF8.GetByteCount(jwt.Key) < 32)
{
    throw new InvalidOperationException(
        "Jwt:Key must be set to at least 32 bytes. Provide it via configuration or the Jwt__Key environment variable.");
}

// Behind a reverse proxy (Caddy/nginx) that terminates TLS: trust the forwarded
// scheme/IP so links and client IPs are correct. The API is only reachable on the
// internal Docker network, so forwarded headers from any proxy are accepted.
builder.Services.Configure<ForwardedHeadersOptions>(o =>
{
    o.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    o.KnownNetworks.Clear();
    o.KnownProxies.Clear();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<UserProvisioningService>();
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
            ClockSkew = TimeSpan.FromMinutes(1),
        };
    });
builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                     ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Ensure the local SQLite data directory exists before the connection is opened.
Directory.CreateDirectory(Path.Combine(app.Environment.ContentRootPath, "data"));

// Apply migrations + seed on startup. Always in development; in production when
// Database:MigrateOnStartup is true (the Droplet deployment sets this, so a single
// `up` provisions/upgrades the schema with no out-of-band step). Idempotent:
// re-running against an already-migrated database is a no-op.
var migrateOnStartup = app.Configuration.GetValue<bool>("Database:MigrateOnStartup");
if (app.Environment.IsDevelopment() || migrateOnStartup)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.MigrateAndSeedAsync(db);

    var seedRelPath = app.Configuration["VideoSeed:Path"] ?? "Seed/video-seed.json";
    var seedPath = Path.Combine(app.Environment.ContentRootPath, seedRelPath);
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("VideoSeeder");
    await VideoSeeder.SeedAsync(db, seedPath, logger);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Honour X-Forwarded-* from the reverse proxy. TLS (and any http->https redirect)
// is handled at the edge by Caddy, so the app itself does not redirect — this
// avoids redirect loops when the container only listens on HTTP.
app.UseForwardedHeaders();

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so the test project (WebApplicationFactory) can reference the entry point.
public partial class Program { }
