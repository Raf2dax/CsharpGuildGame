using System;
using System.Collections.ObjectModel;
using System.Linq;
using GuildGame.Domain.Models;
using GuildGame.Services;
using GuildGame.UI.Infrastructure;

namespace GuildGame.UI.ViewModels;

public class RecruitmentViewModel : ObservableObject
{
    private readonly GameEngine _engine;
    private readonly Random _random;

    public ObservableCollection<Hero> AvailableHeroes { get; }
    public RelayCommand RecruitHeroCommand { get; }

    public RecruitmentViewModel(GameEngine engine)
    {
        _engine = engine;
        _random = new Random();
        AvailableHeroes = new ObservableCollection<Hero>();
        RecruitHeroCommand = new RelayCommand(h => RecruitHero(h as Hero));

        GenerateAvailableHeroes();
    }

    private void GenerateAvailableHeroes()
    {
        AvailableHeroes.Clear();
        for (int i = 0; i < 5; i++)
        {
            var hero = ContentFactory.CreateRandomHero(_random);
            hero.ArrivingDay = 1; // Arrive dans 1 jour
            AvailableHeroes.Add(hero);
        }
    }

    private void RecruitHero(Hero? hero)
    {
        if (hero == null) return;
        _engine.Guild.Heroes.Add(hero);
        // Signal that a hero was recruited
        HeroRecruited?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? HeroRecruited;
}