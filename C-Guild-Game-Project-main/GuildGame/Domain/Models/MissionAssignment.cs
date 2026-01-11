namespace GuildGame.Domain.Models;

public class MissionAssignment
{
    public Mission Mission { get; init; } = new();
    public List<Hero> Heroes { get; init; } = new();
    public bool IsCompleted { get; set; }
    public MissionOutcome? Outcome { get; set; }
}
