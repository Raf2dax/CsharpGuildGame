namespace GuildGame.Domain.Models;

public class Mission
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Mission";
    public string Description { get; set; } = string.Empty;
    public int Difficulty { get; set; } = 1; // 1-5
    public PhaseType Phase { get; set; } = PhaseType.Afternoon;
    public int DurationPhases { get; set; } = 1;
    public bool IsNocturnal => Phase == PhaseType.Evening;
    public ResourceChange Reward { get; set; } = new();
    public HashSet<HeroClass> PreferredClasses { get; set; } = new();
    public RareItem? RareItemReward { get; set; }
    public double RareItemChance { get; set; }
    public List<Hero> AssignedHeroes { get; set; } = new();
    public string AssignedHeroesDisplay => AssignedHeroes.Any() 
        ? string.Join(", ", AssignedHeroes.Select(h => h.Name)) 
        : "Aucun";
}
