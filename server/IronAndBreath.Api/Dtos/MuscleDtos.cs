namespace IronAndBreath.Api.Dtos;

/// <summary>A muscle reference-catalog entry surfaced on the rest screen.</summary>
public record MuscleDto(
    int Id,
    string Name,
    string Group,
    string Function,
    string? InjuryNotes);
