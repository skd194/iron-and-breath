namespace IronAndBreath.Api.Dtos;

public record BodyMetricDto(int Id, DateOnly Date, decimal WeightKg);

public record LogBodyMetricRequest(DateOnly Date, decimal WeightKg);
