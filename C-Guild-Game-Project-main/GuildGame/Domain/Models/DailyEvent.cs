namespace GuildGame.Domain.Models;

public class DailyEvent
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Action<GuildState>? Apply { get; init; }
}
