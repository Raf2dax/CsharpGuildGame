using GuildGame.Domain.Models;

namespace GuildGame.Services;

public class MissionResolver
{
    private readonly Random _random;
    private readonly EventResolver _eventResolver;

    public MissionResolver(Random random)
    {
        _random = random;
        _eventResolver = new EventResolver(random);
    }

    public MissionOutcome Resolve(MissionAssignment assignment, GuildState guild)
    {
        var heroes = assignment.Heroes.Where(h => h.IsAlive).ToList();
        if (!heroes.Any())
        {
            return new MissionOutcome
            {
                Success = false,
                Summary = "Aucun héros valide pour la mission.",
                Reward = new ResourceChange()
            };
        }

        var power = heroes.Sum(h => h.Level) + heroes.Count;
        power += heroes.Sum(h => h.Equipment.Sum(eq => eq.PowerBonus));
        if (heroes.Any(h => h.Traits.HasFlag(Trait.Brave))) power += 2;
        if (heroes.Any(h => h.Traits.HasFlag(Trait.Chanceux))) power += 2;
        if (heroes.Any(h => h.Traits.HasFlag(Trait.Malchanceux))) power -= 1;

        if (assignment.Mission.PreferredClasses.Any())
        {
            var matching = heroes.Count(h => assignment.Mission.PreferredClasses.Contains(h.Class));
            power += matching;
        }

        var difficulty = assignment.Mission.Difficulty * 1.5;
        var chance = Math.Clamp(0.55 + (power - difficulty) * 0.05, 0.10, 0.95);
        var roll = _random.NextDouble();
        var success = roll <= chance;

        var fatigue = 10 + assignment.Mission.Difficulty * 5;
        var hunger = 5 + assignment.Mission.DurationPhases * 5;
        foreach (var hero in heroes)
        {
            hero.ApplyFatigue(fatigue);
            hero.ApplyHunger(hunger);
            hero.IsOnMission = false;
        }

        if (success)
        {
            guild.Resources.Apply(assignment.Mission.Reward);
            RareItem? rareItem = null;
            if (assignment.Mission.RareItemReward != null && assignment.Mission.RareItemChance > 0)
            {
                if (_random.NextDouble() <= assignment.Mission.RareItemChance)
                {
                    rareItem = assignment.Mission.RareItemReward;
                    guild.RareItems.Add(rareItem);
                    ApplyRareItemBuff(rareItem, guild, heroes);
                }
            }
            var rewardStr = $"Or +{assignment.Mission.Reward.Money}, Nourriture +{assignment.Mission.Reward.Food}, Équip +{assignment.Mission.Reward.Equipment}";
            
            // Generate random event
            var randomEvent = _eventResolver.GenerateRandomEvent(guild);
            if (randomEvent != null)
            {
                ApplyRandomEvent(randomEvent, guild, heroes);
            }
            
            return new MissionOutcome
            {
                Success = true,
                Reward = assignment.Mission.Reward,
                Summary = rareItem == null
                    ? $"Succès de {assignment.Mission.Name} (chance {chance:P0}). Récompenses: {rewardStr}"
                    : $"Succès de {assignment.Mission.Name} (chance {chance:P0}). Récompenses: {rewardStr} + Objet rare: {rareItem.Name}",
                Injury = 0,
                HeroDied = false,
                RareItem = rareItem,
                RandomEvent = randomEvent
            };
        }

        var injurySeverity = _random.Next(10, 35) + assignment.Mission.Difficulty * 5;
        var targetHero = heroes[_random.Next(heroes.Count)];
        targetHero.ApplyInjury(injurySeverity);
        var died = targetHero.Health <= 0;

        var penalty = new ResourceChange { Money = -assignment.Mission.Difficulty * 3, Food = 0, Medicine = 0, Equipment = -1 };
        guild.Resources.Apply(penalty);

        var penaltyStr = $"Or -{Math.Abs(penalty.Money)}, Équip -{Math.Abs(penalty.Equipment)}";
        var deathMsg = died ? $" {targetHero.Name} est mort!" : $" {targetHero.Name} a pris {injurySeverity} dégâts.";

        return new MissionOutcome
        {
            Success = false,
            Reward = penalty,
            Summary = $"Échec de {assignment.Mission.Name} (chance {chance:P0}). Pénalités: {penaltyStr}.{deathMsg}",
            Injury = injurySeverity,
            HeroDied = died
        };
    }

    private void ApplyRareItemBuff(RareItem rare, GuildState guild, List<Hero> heroes)
    {
        // Target a random alive hero to receive the buff.
        var candidates = heroes.Where(h => h.IsAlive).ToList();
        if (candidates.Count == 0) candidates = guild.Heroes.Where(h => h.IsAlive).ToList();
        if (candidates.Count == 0) return;

        var target = candidates[_random.Next(candidates.Count)];
        // Equiper automatiquement si place disponible.
        if (target.CanEquip)
        {
            target.Equip(rare);
        }
        switch (rare.BuffType)
        {
            case RareBuffType.Vitalite:
                target.Health = Math.Clamp(target.Health + rare.Magnitude, 0, 120);
                break;
            case RareBuffType.Repos:
                target.ApplyFatigue(-rare.Magnitude);
                break;
            case RareBuffType.Satiete:
                target.ApplyHunger(-rare.Magnitude);
                break;
            case RareBuffType.Chance:
                // Luck buff: add a temporary trait-like effect via Lucky flag.
                target.Traits |= Trait.Chanceux;
                break;
        }
    }

    private void ApplyRandomEvent(RandomEvent randomEvent, GuildState guild, List<Hero> heroes)
    {
        switch (randomEvent.Type)
        {
            case EventType.NewHeroEncounter:
                if (randomEvent.RecruitedHero != null)
                {
                    guild.Heroes.Add(randomEvent.RecruitedHero);
                }
                break;

            case EventType.Ambush:
                foreach (var hero in heroes)
                {
                    hero.ApplyInjury(randomEvent.DamageToHeroes);
                }
                break;

            case EventType.BonusResources:
                if (randomEvent.BonusResources != null)
                {
                    guild.Resources.Apply(randomEvent.BonusResources);
                }
                break;
        }
    }
}
