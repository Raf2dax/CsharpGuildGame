using GuildGame.Domain.Models;

namespace GuildGame.Services.Models;

public class PhaseResult
{
    public PhaseType Phase { get; init; }
    public List<string> Logs { get; } = new();
    public List<MissionAssignment> ResolvedMissions { get; } = new();
    public DailyEvent? TriggeredEvent { get; set; }
    public bool Defeat { get; set; }
    public string DefeatReason { get; set; } = string.Empty;
}
