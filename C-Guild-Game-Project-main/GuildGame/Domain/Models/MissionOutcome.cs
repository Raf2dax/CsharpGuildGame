namespace GuildGame.Domain.Models;

public class MissionOutcome
{
    public bool Success { get; init; }
    public ResourceChange Reward { get; init; } = new();
    public string Summary { get; init; } = string.Empty;
    public int Injury { get; init; }
    public bool HeroDied { get; init; }
    public RareItem? RareItem { get; init; }
    public RandomEvent? RandomEvent { get; init; }
}
