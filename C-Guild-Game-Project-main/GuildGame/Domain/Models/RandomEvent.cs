namespace GuildGame.Domain.Models;

public enum EventType
{
    NewHeroEncounter,
    Ambush,
    BonusResources
}

public class RandomEvent
{
    public EventType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Hero? RecruitedHero { get; set; }
    public ResourceChange? BonusResources { get; set; }
    public int DamageToHeroes { get; set; } // For ambush events
}
