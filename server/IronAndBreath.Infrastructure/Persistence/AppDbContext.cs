using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseVideo> ExerciseVideos => Set<ExerciseVideo>();
    public DbSet<ProgramProgressionPhase> ProgramProgressionPhases => Set<ProgramProgressionPhase>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<SessionSetLog> SessionSetLogs => Set<SessionSetLog>();
    public DbSet<UserProgramSettings> UserProgramSettings => Set<UserProgramSettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<WorkoutDay>(b =>
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Focus).HasMaxLength(200);
            b.HasIndex(x => x.SortOrder);
        });

        modelBuilder.Entity<Exercise>(b =>
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.RepsDisplay).HasMaxLength(60);
            b.Property(x => x.Cue).HasMaxLength(400);

            b.HasOne(x => x.WorkoutDay)
                .WithMany(d => d.Exercises)
                .HasForeignKey(x => x.WorkoutDayId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Video)
                .WithMany(v => v.Exercises)
                .HasForeignKey(x => x.VideoId)
                .OnDelete(DeleteBehavior.SetNull);

            b.HasIndex(x => new { x.WorkoutDayId, x.SortOrder });
        });

        modelBuilder.Entity<ExerciseVideo>(b =>
        {
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.ExternalId).HasMaxLength(100);
            b.Property(x => x.Url).HasMaxLength(500);
            b.Property(x => x.ThumbnailUrl).HasMaxLength(500);
            b.Property(x => x.Attribution).HasMaxLength(300);
            b.Property(x => x.Provider).HasConversion<int>();
        });

        modelBuilder.Entity<ProgramProgressionPhase>(b =>
        {
            b.Property(x => x.RepsHintText).IsRequired().HasMaxLength(200);
            b.Property(x => x.Notes).HasMaxLength(400);
            b.HasIndex(x => x.PhaseNumber).IsUnique();
        });

        modelBuilder.Entity<WorkoutSession>(b =>
        {
            b.HasOne(x => x.WorkoutDay)
                .WithMany(d => d.Sessions)
                .HasForeignKey(x => x.WorkoutDayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes that back the calendar/stats queries.
            b.HasIndex(x => x.Date);
            b.HasIndex(x => x.WorkoutDayId);
        });

        modelBuilder.Entity<SessionSetLog>(b =>
        {
            // Explicit precision so SQLite and PostgreSQL agree on decimal storage.
            b.Property(x => x.WeightKg).HasPrecision(6, 2);

            b.HasOne(x => x.WorkoutSession)
                .WithMany(s => s.SetLogs)
                .HasForeignKey(x => x.WorkoutSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserProgramSettings>(b =>
        {
            b.Property(x => x.DaysPerWeekTarget).HasDefaultValue(4);
        });

        SeedData.Apply(modelBuilder);
    }
}
