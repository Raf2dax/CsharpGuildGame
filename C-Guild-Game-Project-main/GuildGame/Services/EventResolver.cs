using GuildGame.Domain.Models;

namespace GuildGame.Services;

public class EventResolver
{
    private readonly Random _random;

    public EventResolver(Random? random = null)
    {
        _random = random ?? new Random();
    }

    /// <summary>
    /// Generates a random event with 15% chance
    /// </summary>
    public RandomEvent? GenerateRandomEvent(GuildState guild)
    {
        if (_random.Next(100) > 15)
            return null;

        int eventType = _random.Next(3);
        return eventType switch
        {
            0 => GenerateNewHeroEvent(),
            1 => GenerateAmbushEvent(guild),
            2 => GenerateBonusResourceEvent(),
            _ => null
        };
    }

    private RandomEvent GenerateNewHeroEvent()
    {
        var newHero = ContentFactory.CreateRandomHero(_random);
        
        return new RandomEvent
        {
            Type = EventType.NewHeroEncounter,
            Name = "Rencontre d'un nouveau héros",
            Description = $"Vous rencontrez {newHero.Name}, un {newHero.DisplayClass} cherchant à rejoindre votre guilde!",
            RecruitedHero = newHero
        };
    }

    private RandomEvent GenerateAmbushEvent(GuildState guild)
    {
        int damage = _random.Next(5, 15);
        return new RandomEvent
        {
            Type = EventType.Ambush,
            Name = "Embuscade!",
            Description = $"Vos héros ont été pris en embuscade! Tous les héros subissent {damage} dégâts.",
            DamageToHeroes = damage
        };
    }

    private RandomEvent GenerateBonusResourceEvent()
    {
        int gold = _random.Next(20, 50);
        int food = _random.Next(10, 25);
        
        return new RandomEvent
        {
            Type = EventType.BonusResources,
            Name = "Découverte de ressources",
            Description = $"Vous découvrez des ressources cachées! Or +{gold}, Nourriture +{food}",
            BonusResources = new ResourceChange
            {
                Money = gold,
                Food = food
            }
        };
    }
}
