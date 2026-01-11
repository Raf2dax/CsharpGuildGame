namespace GuildGame.Services.Models;

using GuildGame.Domain.Models;

public class HealingService
{
    private readonly ResourceStock _resources;
    
    private static readonly ResourceChange MinorHealCost = new()
    {
        Money = -5,
        Food = 0,
        Medicine = -1,
        Equipment = 0
    };
    
    private static readonly ResourceChange MajorHealCost = new()
    {
        Money = -15,
        Food = 0,
        Medicine = -3,
        Equipment = 0
    };
    
    private static readonly ResourceChange FullHealCost = new()
    {
        Money = -30,
        Food = 0,
        Medicine = -5,
        Equipment = 0
    };
    
    public HealingService(ResourceStock resources)
    {
        _resources = resources;
    }
    
    public bool CanHealHero(Hero hero, HealType healType)
    {
        if (!hero.IsInjured) return false;
        
        var cost = GetHealCost(healType);
        return _resources.CanAfford(cost);
    }
    
    public bool TryHealHero(Hero hero, HealType healType)
    {
        if (!CanHealHero(hero, healType)) return false;
        
        var cost = GetHealCost(healType);
        var healAmount = GetHealAmount(healType);
        
        _resources.Apply(cost);
        hero.Heal(healAmount);
        
        return true;
    }

    private ResourceChange GetHealCost(HealType healType) => healType switch
    {
        HealType.Minor => MinorHealCost,
        HealType.Major => MajorHealCost,
        HealType.Full => FullHealCost,
        _ => throw new ArgumentException("Type de soin invalide")
    };
    
    private int GetHealAmount(HealType healType) => healType switch
    {
        HealType.Minor => 25,
        HealType.Major => 50,
        HealType.Full => 999, 
        _ => 0
    };
}

public enum HealType
{
    Minor,
    Major, 
    Full   
}