using GuildGame.Domain.Models;

namespace GuildGame.Services;

public static class ContentFactory
{
    private static readonly string[] Names =
    [
        "Aria", "Borin", "Cassia", "Darius", "Elly", "Falk", "Galen", "Hex", "Isla", "Joren", "Kira", "Lys", "Mara", "Nox", "Orrin", "Pia", "Quin", "Rhea", "Soren", "Tess",
        "Ulric", "Vexa", "Wynn", "Ysolda", "Zarek", "Anwen", "Brann", "Cleo", "Drystan", "Eira", "Fen", "Gwen", "Hale", "Ilan", "Jade", "Kael", "Leif", "Mika", "Nyra", "Oona"
    ];

    private static RareItem BuildRare(string name, RareBuffType buff, int magnitude, string description, RareItemCategory category, int powerBonus)
    {
        return new RareItem
        {
            Name = name,
            BuffType = buff,
            Magnitude = magnitude,
            Description = description,
            Category = category,
            PowerBonus = powerBonus
        };
    }

    public static RareItem CreateStartingWeapon(Random random)
    {
        return BuildRare("Épée de fer", RareBuffType.Vitalite, 5, "Arme de départ solide", RareItemCategory.Arme, 1);
    }

    public static RareItem CreateStartingArmor(Random random)
    {
        return BuildRare("Cuirasse de cuir", RareBuffType.Repos, 5, "Armure de départ légère", RareItemCategory.Armure, 1);
    }

    public static Hero CreateStartingHero(HeroClass heroClass)
    {
        var baseHealth = GetBaseHealth(heroClass);
        var baseFatigue = GetBaseFatigue(heroClass);
        var salary = GetBaseSalary(heroClass) + 2; // Level 2

        var names = heroClass switch
        {
            HeroClass.Guerrier => "Corin",
            HeroClass.Mage => "Elara",
            HeroClass.Clerc => "Thaddeus",
            _ => Names[0]
        };

        return new Hero
        {
            Name = names,
            Class = heroClass,
            Traits = Trait.Brave,
            Level = 2,
            Salary = salary,
            MaxHealth = baseHealth,
            Health = baseHealth,
            MaxFatigue = baseFatigue,
            Fatigue = 0,
            Hunger = 0
        };
    }

    public static Hero CreateRandomHero(Random random)
    {
        var name = Names[random.Next(Names.Length)];
        var heroClass = (HeroClass)random.Next(Enum.GetValues<HeroClass>().Length);
        var trait = (Trait)(1 << random.Next(0, 6));
        var level = random.Next(1, 4);

        var baseHealth = GetBaseHealth(heroClass);
        var baseFatigue = GetBaseFatigue(heroClass);
        var salary = GetBaseSalary(heroClass) + level;

        return new Hero
        {
            Name = name,
            Class = heroClass,
            Traits = trait,
            Level = level,
            Salary = salary,
            MaxHealth = baseHealth,
            Health = baseHealth,
            MaxFatigue = baseFatigue,
            Fatigue = 0,
            Hunger = 0
        };
    }

    private static int GetBaseHealth(HeroClass heroClass) => heroClass switch
    {
        HeroClass.Guerrier => 110,
        HeroClass.Paladin => 105,
        HeroClass.Berserker => 115,
        HeroClass.Rodeur => 95,
        HeroClass.Voleur => 90,
        HeroClass.Mage => 80,
        HeroClass.Clerc => 95,
        _ => 90
    };

    private static int GetBaseFatigue(HeroClass heroClass) => heroClass switch
    {
        HeroClass.Guerrier => 15,
        HeroClass.Paladin => 15,
        HeroClass.Berserker => 20,
        HeroClass.Rodeur => 10,
        HeroClass.Voleur => 10,
        HeroClass.Mage => 12,
        HeroClass.Clerc => 12,
        _ => 10
    };

    private static int GetBaseSalary(HeroClass heroClass) => heroClass switch
    {
        HeroClass.Guerrier or HeroClass.Paladin or HeroClass.Berserker => 8,
        HeroClass.Rodeur or HeroClass.Voleur => 7,
        HeroClass.Mage or HeroClass.Clerc => 7,
        _ => 6
    };

    public static Hero CreateHero(Random random)
    {
        var name = Names[random.Next(Names.Length)];
        var heroClass = (HeroClass)random.Next(Enum.GetValues<HeroClass>().Length);
        var trait = (Trait)(1 << random.Next(0, 6));
        var level = random.Next(1, 4);

        var baseHealth = GetBaseHealth(heroClass);
        var baseFatigue = GetBaseFatigue(heroClass);
        var salary = GetBaseSalary(heroClass) + level;

        return new Hero
        {
            Name = name,
            Class = heroClass,
            Traits = trait,
            Level = level,
            Salary = salary,
            MaxHealth = baseHealth,
            Health = baseHealth,
            MaxFatigue = baseFatigue,
            Fatigue = baseFatigue,
            Hunger = 0
        };
    }

    public static List<Mission> GenerateMissions(Random random, int day)
    {
        var difficultyBias = Math.Clamp(1 + day / 3, 1, 5);
        var templates = new List<Func<Mission>>
        {
            () => new Mission
            {
                Name = "Escorte marchande",
                Description = "Protéger un convoi sur des routes incertaines.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(-1, 1), 1, 5),
                Phase = PhaseType.Afternoon,
                DurationPhases = 1,
                Reward = new ResourceChange { Money = 10 + random.Next(0, 6), Food = random.Next(1, 4) },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Voleur, HeroClass.Rodeur }
            },
            () => new Mission
            {
                Name = "Chasse au basilic",
                Description = "Chasser une bête rare pour obtenir des composants.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(0, 2), 2, 5),
                Phase = PhaseType.Afternoon,
                DurationPhases = 1,
                Reward = new ResourceChange { Money = 6 + random.Next(0, 5), Medicine = 1 + random.Next(0, 3), Equipment = 1 },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Rodeur, HeroClass.Voleur },
                RareItemReward = BuildRare("Écaille de basilic", RareBuffType.Vitalite, 12, "+12 vitalité", RareItemCategory.Armure, 2),
                RareItemChance = 0.20
            },
            () => new Mission
            {
                Name = "Rituels nocturnes",
                Description = "Briser un rituel obscur avant l'aube.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(1, 3), 2, 5),
                Phase = PhaseType.Evening,
                DurationPhases = 1,
                Reward = new ResourceChange { Money = 12 + random.Next(0, 8), Medicine = random.Next(0, 2) },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Mage, HeroClass.Clerc, HeroClass.Paladin },
                RareItemReward = BuildRare("Fragment runique", RareBuffType.Chance, 2, "+2 chance", RareItemCategory.Accessoire, 2),
                RareItemChance = 0.25
            },
            () => new Mission
            {
                Name = "Expédition de ruines",
                Description = "Explorer des ruines anciennes à la recherche d'artefacts.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(-1, 2), 1, 5),
                Phase = PhaseType.Afternoon,
                DurationPhases = 2,
                Reward = new ResourceChange { Money = 14 + random.Next(0, 10), Equipment = 2 },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Voleur, HeroClass.Rodeur },
                RareItemReward = BuildRare("Relique oubliée", RareBuffType.Repos, 10, "+10 repos", RareItemCategory.Accessoire, 1),
                RareItemChance = 0.30
            },
            () => new Mission
            {
                Name = "Patrouille de frontière",
                Description = "Sécuriser les routes pour les voyageurs.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(-2, 1), 1, 4),
                Phase = PhaseType.Afternoon,
                DurationPhases = 1,
                Reward = new ResourceChange { Money = 8 + random.Next(0, 4), Food = random.Next(0, 3) },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Guerrier, HeroClass.Paladin }
            },
            () => new Mission
            {
                Name = "Infiltration de camp",
                Description = "Saboter un camp ennemi pendant la nuit.",
                Difficulty = Math.Clamp(difficultyBias + random.Next(0, 3), 2, 5),
                Phase = PhaseType.Evening,
                DurationPhases = 1,
                Reward = new ResourceChange { Money = 16 + random.Next(0, 6), Equipment = 1 + random.Next(0, 2) },
                PreferredClasses = new HashSet<HeroClass> { HeroClass.Voleur, HeroClass.Berserker },
                RareItemReward = BuildRare("Lame silencieuse", RareBuffType.Satiete, 12, "+12 satiété", RareItemCategory.Arme, 3),
                RareItemChance = 0.18
            }
        };

        var missions = new List<Mission>();
        var missionCount = 5;
        for (int i = 0; i < missionCount; i++)
        {
            var template = templates[random.Next(templates.Count)];
            var mission = template();
            // Small scaling with day
            mission.Difficulty = Math.Clamp(mission.Difficulty + random.Next(0, day / 5 + 1), 1, 5);
            mission.Reward.Money += day / 4;
            missions.Add(mission);
        }

        return missions;
    }

    public static List<DailyEvent> BuildEvents(Random random)
    {
        return new List<DailyEvent>
        {
            new()
            {
                Title = "Marchand généreux",
                Description = "Un marchand offre quelques vivres.",
                Apply = guild => guild.Resources.Apply(new ResourceChange { Food = 4, Money = -2 })
            },
            new()
            {
                Title = "Visite de mécène",
                Description = "Un noble paie pour soutenir la guilde.",
                Apply = guild => guild.Resources.Apply(new ResourceChange { Money = 12 })
            },
            new()
            {
                Title = "Embuscade",
                Description = "Les héros sont tendus, ils se fatiguent.",
                Apply = guild =>
                {
                    foreach (var hero in guild.Heroes)
                    {
                        hero.ApplyFatigue(5);
                    }
                }
            }
        };
    }
}
