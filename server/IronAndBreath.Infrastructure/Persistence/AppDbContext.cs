using IronAndBreath.Domain.Entities;
using IronAndBreath.Infrastructure.Persistence.Seed;
using Microsoft.EntityFrameworkCore;

namespace IronAndBreath.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<WorkoutDay> WorkoutDays => Set<WorkoutDay>();
    public DbSet<Exercise> Exercises => Set<Exercise>();
    public DbSet<ExerciseVideo> ExerciseVideos => Set<ExerciseVideo>();
    public DbSet<ProgramProgressionPhase> ProgramProgressionPhases => Set<ProgramProgressionPhase>();
    public DbSet<WorkoutSession> WorkoutSessions => Set<WorkoutSession>();
    public DbSet<SessionSetLog> SessionSetLogs => Set<SessionSetLog>();
    public DbSet<UserProgramSettings> UserProgramSettings => Set<UserProgramSettings>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(b =>
        {
            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.DisplayName).IsRequired().HasMaxLength(120);
            b.Property(x => x.PasswordHash).HasMaxLength(400);
            b.Property(x => x.GoogleSubject).HasMaxLength(64);
            b.HasIndex(x => x.Email).IsUnique();
            b.HasIndex(x => x.GoogleSubject).IsUnique();
        });

        modelBuilder.Entity<WorkoutDay>(b =>
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(100);
            b.Property(x => x.Focus).HasMaxLength(200);
            b.HasIndex(x => x.SortOrder);

            // Null UserId = seeded template. User-owned days cascade-delete with the user.
            b.HasOne(x => x.User)
                .WithMany(u => u.WorkoutDays)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<Exercise>(b =>
        {
            b.Property(x => x.Name).IsRequired().HasMaxLength(120);
            b.Property(x => x.RepsDisplay).HasMaxLength(60);
            b.Property(x => x.Cue).HasMaxLength(400);

            // Coaching metadata.
            b.Property(x => x.BreathingConcentric).HasMaxLength(60);
            b.Property(x => x.BreathingEccentric).HasMaxLength(60);
            b.Property(x => x.BreathingNotes).HasMaxLength(200);
            b.Property(x => x.PrimaryMuscles).HasMaxLength(200);
            b.Property(x => x.SecondaryMuscles).HasMaxLength(200);
            b.Property(x => x.Tempo).HasMaxLength(60);
            b.Property(x => x.Benefits).HasMaxLength(600);
            b.Property(x => x.CommonMistakes).HasMaxLength(600);
            b.Property(x => x.SafetyTips).HasMaxLength(600);
            b.Property(x => x.AnimationRef).HasMaxLength(120);

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
            b.Property(x => x.Source).HasConversion<int>();
            b.Property(x => x.Notes).HasMaxLength(1000);

            // Optional day: null for ad-hoc manual sessions. Restrict so a day
            // with logged sessions can't be silently deleted.
            b.HasOne(x => x.WorkoutDay)
                .WithMany(d => d.Sessions)
                .HasForeignKey(x => x.WorkoutDayId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes that back the calendar/stats queries.
            b.HasIndex(x => x.Date);
            b.HasIndex(x => x.WorkoutDayId);
            b.HasIndex(x => new { x.UserId, x.Date });
        });

        modelBuilder.Entity<SessionSetLog>(b =>
        {
            // Explicit precision so SQLite and PostgreSQL agree on decimal storage.
            b.Property(x => x.WeightKg).HasPrecision(6, 2);
            b.Property(x => x.ExerciseName).HasMaxLength(120);
            b.Property(x => x.Notes).HasMaxLength(400);

            b.HasOne(x => x.WorkoutSession)
                .WithMany(s => s.SetLogs)
                .HasForeignKey(x => x.WorkoutSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Optional exercise link. SetNull keeps the historical set (with its
            // ExerciseName) if the linked exercise is later deleted.
            b.HasOne(x => x.Exercise)
                .WithMany()
                .HasForeignKey(x => x.ExerciseId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<UserProgramSettings>(b =>
        {
            b.Property(x => x.DaysPerWeekTarget).HasDefaultValue(4);
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.UserId).IsUnique();
        });

        modelBuilder.Entity<Conversation>(b =>
        {
            b.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.UserId);
        });

        modelBuilder.Entity<ChatMessage>(b =>
        {
            b.Property(x => x.Role).HasConversion<int>();
            b.Property(x => x.Content).IsRequired().HasMaxLength(8000);

            b.HasOne(x => x.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(x => x.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(x => x.ConversationId);
        });

        SeedData.Apply(modelBuilder);
    }
}
