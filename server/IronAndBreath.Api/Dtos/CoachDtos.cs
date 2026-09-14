namespace IronAndBreath.Api.Dtos;

public record CoachMessageDto(int Id, string Role, string Content, DateTimeOffset CreatedAt);

public record CoachStateDto(bool AiEnabled, string Provider, IReadOnlyList<CoachMessageDto> Messages);

public record SendMessageRequest(string Content);
