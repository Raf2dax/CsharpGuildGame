using GuildGame.Domain.Models;
using GuildGame.Services.Models;

namespace GuildGame.Services;

public class GameEngine
{
    public GuildState Guild { get; }
    public List<Mission> Missions { get; private set; }

    private readonly MissionResolver _resolver;
    private readonly Random _random;
    private readonly List<DailyEvent> _events;
    private readonly HealingService _healingService;
    
    public GameEngine(GuildState? guild = null, Random? random = null)
    {
        _random = random ?? new Random();
        Guild = guild ?? new GuildState();
        _resolver = new MissionResolver(_random);
        _events = ContentFactory.BuildEvents(_random);
        _healingService = new HealingService(Guild.Resources);
        
        if (!Guild.Heroes.Any())
        {
            Guild.Heroes.Add(ContentFactory.CreateStartingHero(HeroClass.Guerrier));
            Guild.Heroes.Add(ContentFactory.CreateStartingHero(HeroClass.Mage));
            Guild.Heroes.Add(ContentFactory.CreateStartingHero(HeroClass.Clerc));
        }

        if (!Guild.RareItems.Any())
        {
            Guild.RareItems.Add(ContentFactory.CreateStartingWeapon(_random));
            Guild.RareItems.Add(ContentFactory.CreateStartingArmor(_random));
        }

        Missions = ContentFactory.GenerateMissions(_random, Guild.Day);
    }

    public void RefreshMissions()
    {
        Missions = ContentFactory.GenerateMissions(_random, Guild.Day);
    }

    public PhaseResult RunPhase(PhaseType phase, IEnumerable<MissionAssignment>? assignments = null)
    {
        var result = new PhaseResult { Phase = phase };
        assignments ??= Enumerable.Empty<MissionAssignment>();

        switch (phase)
        {
            case PhaseType.Morning:
                RefreshMissions();
                HandleMorning(result);
                break;
            case PhaseType.Afternoon:
                HandleMissions(assignments.Where(a => a.Mission.Phase == PhaseType.Afternoon), result);
                MaybeTriggerEvent(result);
                break;
            case PhaseType.Evening:
                HandleMissions(assignments.Where(a => a.Mission.Phase == PhaseType.Evening), result);
                HandleEveningRecovery(result);
                Guild.NextDay();
                break;
        }

        if (Guild.CheckDefeat(out var reason))
        {
            result.Defeat = true;
            result.DefeatReason = reason;
        }

        return result;
    }

    private void HandleMorning(PhaseResult result)
    {
        // Décrémente les jours d'arrivée des héros
        foreach (var hero in Guild.Heroes.Where(h => h.ArrivingDay.HasValue && h.ArrivingDay > 0))
        {
            hero.ArrivingDay--;
            if (hero.ArrivingDay <= 0)
            {
                result.Logs.Add($"{hero.Name} est arrivé à la guilde!");
            }
        }

        // Consommation de nourriture et salaires.
        var foodCost = Guild.Heroes.Count(h => h.IsAlive && h.IsAvailable);
        if (Guild.Resources.Food >= foodCost)
        {
            Guild.Resources.Apply(new ResourceChange { Food = -foodCost });
            result.Logs.Add($"Nourriture distribuée (-{foodCost}).");
        }
        else
        {
            foreach (var hero in Guild.Heroes)
            {
                hero.ApplyHunger(15);
            }
            result.Logs.Add("Nourriture insuffisante, les héros ont faim.");
        }

        var salaryCost = Guild.Heroes.Where(h => h.IsAvailable).Sum(h => h.Salary);
        Guild.Resources.Apply(new ResourceChange { Money = -salaryCost });
        if (Guild.Resources.Money < 0)
        {
            Guild.AddDebt(Math.Abs(Guild.Resources.Money));
        }
        result.Logs.Add($"Salaires payés (-{salaryCost}). Dette: {Guild.Debt}.");

        // Retour de missions nocturnes simplifié : on libère les héros.
        foreach (var hero in Guild.Heroes)
        {
            hero.IsOnMission = false;
        }
    }

    private void HandleMissions(IEnumerable<MissionAssignment> assignments, PhaseResult result)
    {
        foreach (var assignment in assignments)
        {
            foreach (var hero in assignment.Heroes)
            {
                hero.IsOnMission = true;
            }

            var outcome = _resolver.Resolve(assignment, Guild);
            assignment.Outcome = outcome;
            assignment.IsCompleted = true;
            result.ResolvedMissions.Add(assignment);
            result.Logs.Add(outcome.Summary);
        }
    }

    private void MaybeTriggerEvent(PhaseResult result)
    {
        if (_events.Count == 0) return;
        var roll = _random.NextDouble();
        if (roll < 0.35)
        {
            var ev = _events[_random.Next(_events.Count)];
            ev.Apply?.Invoke(Guild);
            result.TriggeredEvent = ev;
            result.Logs.Add($"Événement : {ev.Title}.");
        }
    }

    private void HandleEveningRecovery(PhaseResult result)
    {
        foreach (var hero in Guild.Heroes)
        {
            if (!hero.IsOnMission)
            {
                hero.Rest();
            }
        }
        result.Logs.Add("Fin de journée : repos et récupération légère.");
    }
    
    public HealingService HealingService => _healingService;
}
