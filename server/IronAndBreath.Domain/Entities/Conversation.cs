namespace IronAndBreath.Domain.Entities;

/// <summary>
/// A user's persistent AI-coach conversation. Kept across plan generation and
/// workouts so the coach retains context. One active conversation per user for now.
/// </summary>
public class Conversation
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
}

/// <summary>Who authored a chat message.</summary>
public enum ChatRole
{
    User = 0,
    Assistant = 1,
}

/// <summary>A single message in a <see cref="Conversation"/>.</summary>
public class ChatMessage
{
    public int Id { get; set; }

    public int ConversationId { get; set; }
    public Conversation? Conversation { get; set; }

    public ChatRole Role { get; set; }

    public string Content { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }
}
