namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A dated bodyweight measurement, used to chart the physique trend over the
/// program. One row per user per logged day (latest wins on the same date).
/// </summary>
public class BodyMetric
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateOnly Date { get; set; }

    public decimal WeightKg { get; set; }
}
