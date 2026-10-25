namespace IronAndBreath.Api.Dtos;

/// <summary>A piece of equipment in the user's inventory. Category/Status as strings.</summary>
public record EquipmentItemDto(
    int Id,
    string Name,
    string Category,
    string? Specs,
    string Status,
    int SortOrder);

public record UpsertEquipmentRequest(
    string Name,
    string Category,
    string? Specs,
    string Status);
