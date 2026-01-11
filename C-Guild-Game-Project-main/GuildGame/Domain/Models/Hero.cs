namespace GuildGame.Domain.Models;

public class Hero
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public HeroClass Class { get; set; }
    public Trait Traits { get; set; }
    public int Level { get; set; } = 1;
    public int MaxHealth { get; set; } = 100;
    public int Health { get; set; } = 100; // 0-100
    public int Fatigue { get; set; } = 0;  // 0-100
    public int Hunger { get; set; } = 0;   // 0-100
    public int Salary { get; set; } = 5;
    public int MaxFatigue { get; set; } = 100;
    public bool IsOnMission { get; set; }
    public int? ArrivingDay { get; set; } = null;
    public List<RareItem> Equipment { get; } = new();

    public bool IsAlive => Health > 0;
    public bool IsExhausted => Fatigue >= 100 || Hunger >= 100 || Health <= 20;
    public bool IsAvailable => ArrivingDay == null || ArrivingDay <= 0;
    
    public bool IsInjured => Health < MaxHealth;

    public string DisplayClass => Class.ToString();
    public string DisplayTraits => Traits.ToDisplayString();
    public string DisplayLabel => $"{Name} (Niv {Level})";
    public string ArrivingDayDisplay => ArrivingDay.HasValue && ArrivingDay > 0 ? $"Arrivera dans {ArrivingDay} jour(s)" : "";

    public bool CanEquip => Equipment.Count < 3;

    public void Rest()
    {
        Fatigue = Math.Max(0, Fatigue - 20);
        Hunger = Math.Max(0, Hunger - 10);
        IsOnMission = false;
    }

    public bool Equip(RareItem item)
    {
        if (!CanEquip) return false;
        Equipment.Add(item);
        return true;
    }

    public bool Unequip(RareItem item)
    {
        return Equipment.Remove(item);
    }

    public void ApplyInjury(int severity)
    {
        Health = Math.Max(0, Health - severity);
    }

    public void ApplyFatigue(int amount)
    {
        Fatigue = Math.Clamp(Fatigue + amount, 0, 120);
    }

    public void ApplyHunger(int amount)
    {
        Hunger = Math.Clamp(Hunger + amount, 0, 120);
    }
    
    public void Heal(int amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
}