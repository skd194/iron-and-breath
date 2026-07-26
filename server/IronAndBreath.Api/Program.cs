using IronAndBreath.Api.Services;
using IronAndBreath.Infrastructure;
using IronAndBreath.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "ClientApp";

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IVideoResolver, VideoResolver>();
builder.Services.Configure<WarmUpOptions>(builder.Configuration.GetSection(WarmUpOptions.SectionName));

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

// Apply migrations + seed on startup in development. Guarded so production
// deployments can run migrations out-of-band instead.
if (app.Environment.IsDevelopment())
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

// Skip HTTPS redirection in development so the Vite proxy can reach the API
// over plain HTTP without a 307 dance through the dev certificate.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicy);
app.UseAuthorization();
app.MapControllers();

app.Run();

// Exposed so the test project (WebApplicationFactory) can reference the entry point.
public partial class Program { }
