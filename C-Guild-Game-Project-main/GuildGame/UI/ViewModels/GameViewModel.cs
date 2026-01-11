using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using GuildGame.Domain.Models;
using GuildGame.Services;
using GuildGame.Services.Models;
using GuildGame.UI.Infrastructure;

namespace GuildGame.UI.ViewModels;

public class GameViewModel : ObservableObject
{
    private GameEngine _engine;
    private MerchantService _merchant;
    private PhaseType _lastPhase = PhaseType.Evening;

    public ObservableCollection<Hero> Heroes { get; }
    public ObservableCollection<Mission> Missions { get; }
    public ObservableCollection<MissionAssignment> PendingAssignments { get; }
    public ObservableCollection<MerchantOffer> Offers { get; }
    public ObservableCollection<string> Logs { get; }
    public ObservableCollection<RareItem> RareItems { get; }
    public ObservableCollection<RareItem> EquippedItems { get; }

    private Hero? _selectedHero;
    public Hero? SelectedHero
    {
        get => _selectedHero;
        set
        {
            _selectedHero = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedHeroStats));
            OnPropertyChanged(nameof(SelectedHeroTraits));
            OnPropertyChanged(nameof(SelectedHeroEquipment));
        }
    }

    private string _defeatMessage = string.Empty;
    public string DefeatMessage
    {
        get => _defeatMessage;
        private set
        {
            _defeatMessage = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand RunMorningCommand { get; }
    public RelayCommand RunAfternoonCommand { get; }
    public RelayCommand RunEveningCommand { get; }
    public RelayCommand BuyOfferCommand { get; }
    public RelayCommand RefreshMissionsCommand { get; }
    public RelayCommand SelectHeroCommand { get; }
    public RelayCommand EquipItemCommand { get; }
    public RelayCommand UnequipItemCommand { get; }
    public RelayCommand AssignMissionCommand { get; }
    public RelayCommand RecruitCommand { get; }
    public RelayCommand CancelMissionCommand { get; }
    public RelayCommand OpenMerchantCommand { get; }
    public RelayCommand RestartCommand { get; }
    public RelayCommand HealMinorCommand { get; }
    public RelayCommand HealMajorCommand { get; }
    public RelayCommand HealFullCommand { get; }

    private string _statusBanner = "Guilde en marche";
    public string StatusBanner
    {
        get => _statusBanner;
        private set
        {
            _statusBanner = value;
            OnPropertyChanged();
        }
    }

    private string _currentPhase = "Matin";
    public string CurrentPhase
    {
        get => _currentPhase;
        private set
        {
            _currentPhase = value;
            OnPropertyChanged();
        }
    }

    private Brush _bannerBrush = Brushes.DimGray;
    public Brush BannerBrush
    {
        get => _bannerBrush;
        private set
        {
            _bannerBrush = value;
            OnPropertyChanged();
        }
    }

    private string _healStatusMessage = string.Empty;
    public string HealStatusMessage
    {
        get => _healStatusMessage;
        private set
        {
            _healStatusMessage = value;
            OnPropertyChanged();
        }
    }

    private Brush _healStatusColor = Brushes.Gray;
    public Brush HealStatusColor
    {
        get => _healStatusColor;
        private set
        {
            _healStatusColor = value;
            OnPropertyChanged();
        }
    }

    public GameViewModel()
    {
        _engine = new GameEngine();
        _merchant = new MerchantService(_engine.Guild);

        Heroes = new ObservableCollection<Hero>(_engine.Guild.Heroes);
        Missions = new ObservableCollection<Mission>(_engine.Missions);
        PendingAssignments = new ObservableCollection<MissionAssignment>();
        Offers = new ObservableCollection<MerchantOffer>(_merchant.GetDailyOffers());
        Logs = new ObservableCollection<string>();
        RareItems = new ObservableCollection<RareItem>(_engine.Guild.RareItems);
        EquippedItems = new ObservableCollection<RareItem>();

        RunMorningCommand = new RelayCommand(_ => RunPhase(PhaseType.Morning));
        RunAfternoonCommand = new RelayCommand(_ => RunPhase(PhaseType.Afternoon));
        RunEveningCommand = new RelayCommand(_ => RunPhase(PhaseType.Evening));
        BuyOfferCommand = new RelayCommand(o => BuyOffer(o as MerchantOffer));
        RefreshMissionsCommand = new RelayCommand(_ => RefreshMissions());
        SelectHeroCommand = new RelayCommand(h => SelectedHero = h as Hero);
        EquipItemCommand = new RelayCommand(i => EquipItem(i as RareItem));
        UnequipItemCommand = new RelayCommand(i => UnequipItem(i as RareItem));
        AssignMissionCommand = new RelayCommand(AssignSelectedMission);
        RecruitCommand = new RelayCommand(_ => OpenRecruitmentWindow());
        CancelMissionCommand = new RelayCommand(assignment => CancelMissionAssignment(assignment as MissionAssignment));
        OpenMerchantCommand = new RelayCommand(_ => OpenMerchantWindow());
        RestartCommand = new RelayCommand(_ => Restart());
        HealMinorCommand = new RelayCommand(_ => HealSelectedHero(HealType.Minor));
        HealMajorCommand = new RelayCommand(_ => HealSelectedHero(HealType.Major));
        HealFullCommand = new RelayCommand(_ => HealSelectedHero(HealType.Full));

        _lastPhase = PhaseType.Evening; // Allow Morning to be first
        RunPhase(PhaseType.Morning); // Start the game with the morning phase
    }

    public int Money => _engine.Guild.Resources.Money;
    public int Food => _engine.Guild.Resources.Food;
    public int Medicine => _engine.Guild.Resources.Medicine;
    public int Equipment => _engine.Guild.Resources.Equipment;
    public int Debt => _engine.Guild.Debt;
    public int Day => _engine.Guild.Day;

    public string SelectedHeroStats => SelectedHero == null
        ? "Sélectionnez un héros"
        : $"PV {SelectedHero.Health}/{SelectedHero.MaxHealth} | Fatigue {SelectedHero.Fatigue}/{SelectedHero.MaxFatigue} | Faim {SelectedHero.Hunger}/100 | Niveau {SelectedHero.Level} | Salaire {SelectedHero.Salary} | Statut: {(SelectedHero.IsOnMission ? "Occupé" : "Libre")}{(SelectedHero.ArrivingDay.HasValue && SelectedHero.ArrivingDay > 0 ? $" | {SelectedHero.ArrivingDayDisplay}" : "")};";

    public string SelectedHeroTraits => SelectedHero == null
        ? string.Empty
        : $"Classe: {SelectedHero.DisplayClass} | Traits: {SelectedHero.DisplayTraits}";

    public IEnumerable<RareItem> SelectedHeroEquipment => SelectedHero?.Equipment ?? Enumerable.Empty<RareItem>();

    private Mission? _selectedMission;
    public Mission? SelectedMission
    {
        get => _selectedMission;
        set
        {
            _selectedMission = value;
            OnPropertyChanged();
        }
    }

    private void AssignSelectedMission(object? parameter)
    {
        if (SelectedMission == null) return;
        var selectedHeroes = Heroes.Where(h => h.IsAlive && !h.IsOnMission && !h.IsExhausted).ToList();
        if (!selectedHeroes.Any()) return;
        
        AssignMission(SelectedMission, selectedHeroes);
        SelectedMission = null;
    }

    public void AssignMission(Mission mission, IList<Hero> heroes)
    {
        var cleanHeroes = heroes.OfType<Hero>().Where(h => h.IsAlive && !h.IsOnMission).ToList();
        if (!cleanHeroes.Any()) return;

        mission.AssignedHeroes = cleanHeroes;
        foreach (var hero in cleanHeroes)
        {
            hero.IsOnMission = true;
        }

        PendingAssignments.Add(new MissionAssignment
        {
            Mission = mission,
            Heroes = cleanHeroes
        });
        
        // Remove mission from available missions list
        Missions.Remove(mission);
        
        Logs.Add($"Mission '{mission.Name}' assignée à {string.Join(", ", cleanHeroes.Select(h => h.Name))}");
    }

    private void RunPhase(PhaseType phase)
    {
        // Validate phase order: Morning -> Afternoon -> Evening -> Morning
        var validPhaseOrder = new Dictionary<PhaseType, PhaseType[]>
        {
            { PhaseType.Morning, new[] { PhaseType.Afternoon } },
            { PhaseType.Afternoon, new[] { PhaseType.Evening } },
            { PhaseType.Evening, new[] { PhaseType.Morning } }
        };

        if (!validPhaseOrder[_lastPhase].Contains(phase))
        {
            Logs.Add($"Vous devez faire la phase {(PhaseType)(((int)_lastPhase + 1) % 3)} avant celle-ci.");
            return;
        }

        _lastPhase = phase;
        UpdateCurrentPhaseDisplay(phase);

        if (!string.IsNullOrEmpty(DefeatMessage)) return;

        var assignments = PendingAssignments.Where(a => a.Mission.Phase == phase).ToList();
        foreach (var a in assignments)
        {
            PendingAssignments.Remove(a);
        }

        var result = _engine.RunPhase(phase, assignments);
        ApplyPhaseResult(result);
    }

    private void UpdateCurrentPhaseDisplay(PhaseType phase)
    {
        CurrentPhase = phase switch
        {
            PhaseType.Morning => "Matin",
            PhaseType.Afternoon => "Après-midi",
            PhaseType.Evening => "Soir",
            _ => "Inconnue"
        };
    }

    private void ApplyPhaseResult(PhaseResult result)
    {
        foreach (var log in result.Logs)
        {
            Logs.Add($"Jour {Day} - {result.Phase}: {log}");
        }

        if (result.ResolvedMissions.Any())
        {
            RefreshHeroes();
            
            // Display random events from missions
            foreach (var resolved in result.ResolvedMissions)
            {
                if (resolved.Outcome?.RandomEvent != null)
                {
                    var evt = resolved.Outcome.RandomEvent;
                    Logs.Add($"✦ ÉVÉNEMENT: {evt.Name} - {evt.Description}");
                    
                    // If new hero was recruited, add to heroes list
                    if (evt.RecruitedHero != null && !Heroes.Contains(evt.RecruitedHero))
                    {
                        Heroes.Add(evt.RecruitedHero);
                    }
                }
            }
        }

        if (result.TriggeredEvent != null)
        {
            Logs.Add($"Événement: {result.TriggeredEvent.Title}");
        }

        OnPropertyChanged(nameof(Money));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Medicine));
        OnPropertyChanged(nameof(Equipment));
        OnPropertyChanged(nameof(Debt));
        OnPropertyChanged(nameof(Day));

        foreach (var resolved in result.ResolvedMissions)
        {
            if (resolved.Outcome?.RareItem != null)
            {
                RareItems.Add(resolved.Outcome.RareItem);
                Logs.Add($"Butin rare obtenu : {resolved.Outcome.RareItem.Name}");
            }
        }

        if (result.Defeat)
        {
            DefeatMessage = result.DefeatReason;
            Logs.Add($"Défaite: {result.DefeatReason}");
            UpdateBanner(defeated: true);
        }
        else
        {
            UpdateBanner(defeated: false);
        }
    }

    private void UpdateBanner(bool defeated)
    {
        if (defeated)
        {
            StatusBanner = "Défaite";
            BannerBrush = Brushes.Firebrick;
            return;
        }

        var prosperity = Money >= 150 && Food >= 60 && Debt <= 0;
        if (prosperity)
        {
            StatusBanner = "Guilde prospère !";
            BannerBrush = Brushes.SeaGreen;
        }
        else
        {
            StatusBanner = "Aventure en cours";
            BannerBrush = Brushes.SteelBlue;
        }
    }

    private void RefreshHeroes()
    {
        Heroes.Clear();
        foreach (var hero in _engine.Guild.Heroes)
        {
            Heroes.Add(hero);
        }
        OnPropertyChanged(nameof(SelectedHeroEquipment));
    }

    private void RefreshMissions()
    {
        _engine.RefreshMissions();
        Missions.Clear();
        foreach (var mission in _engine.Missions)
        {
            Missions.Add(mission);
        }
    }

    private void BuyOffer(MerchantOffer? offer)
    {
        if (offer == null) return;
        var ok = _merchant.ExecuteOffer(offer);
        if (ok && offer.RecruitFactory != null)
        {
            Heroes.Add(_engine.Guild.Heroes.Last());
        }
        RefreshResourceBindings();
    }

    private void RefreshResourceBindings()
    {
        OnPropertyChanged(nameof(Money));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Medicine));
        OnPropertyChanged(nameof(Equipment));
        OnPropertyChanged(nameof(Debt));
    }

    private void EquipItem(RareItem? item)
    {
        if (item == null || SelectedHero == null) return;
        if (!SelectedHero.CanEquip) { Logs.Add($"{SelectedHero.Name} ne peut porter plus d'équipements."); return; }

        var removed = RareItems.Remove(item);
        if (!removed)
        {
            // L'item peut déjà être en inventaire interne mais non visible (sécurité)
            if (!_engine.Guild.RareItems.Contains(item)) return;
            _engine.Guild.RareItems.Remove(item);
        }
        SelectedHero.Equip(item);
        _engine.Guild.RareItems.Remove(item);
        Logs.Add($"{item.Name} équipé sur {SelectedHero.Name}.");
        OnPropertyChanged(nameof(SelectedHeroEquipment));
    }

    private void UnequipItem(RareItem? item)
    {
        if (item == null || SelectedHero == null) return;
        if (!SelectedHero.Unequip(item)) return;
        
        RareItems.Add(item);
        _engine.Guild.RareItems.Add(item);
        Logs.Add($"{item.Name} retiré de {SelectedHero.Name}.");
        OnPropertyChanged(nameof(SelectedHeroEquipment));
    }

    private void OpenRecruitmentWindow()
    {
        var viewModel = new RecruitmentViewModel(_engine);
        var window = new RecruitmentWindow(viewModel)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };
        window.ShowDialog();
        RefreshHeroes();
    }

    private void OpenMerchantWindow()
    {
        var window = new MerchantWindow(this)
        {
            Owner = System.Windows.Application.Current.MainWindow
        };
        window.ShowDialog();
    }

    private void CancelMissionAssignment(MissionAssignment? assignment)
    {
        if (assignment == null) return;
        
        // Mark heroes as no longer on mission
        foreach (var hero in assignment.Heroes)
        {
            hero.IsOnMission = false;
        }
        
        // Remove from pending assignments
        PendingAssignments.Remove(assignment);
        
        // Add mission back to available missions
        Missions.Add(assignment.Mission);
        
        Logs.Add($"Mission '{assignment.Mission.Name}' annulée.");
    }

    private void HealSelectedHero(HealType healType)
    {
        if (SelectedHero == null)
        {
            HealStatusMessage = "Aucun héros sélectionné.";
            HealStatusColor = Brushes.Red;
            return;
        }

        if (!SelectedHero.IsInjured)
        {
            HealStatusMessage = $"{SelectedHero.Name} n'est pas blessé.";
            HealStatusColor = Brushes.Orange;
            return;
        }

        var healTypeName = healType switch
        {
            HealType.Minor => "Léger",
            HealType.Major => "Moyen",
            HealType.Full => "Complet",
            _ => "Inconnu"
        };

        if (_engine.HealingService.TryHealHero(SelectedHero, healType))
        {
            HealStatusMessage = $"{SelectedHero.Name} soigné ({healTypeName}) ! PV: {SelectedHero.Health}/{SelectedHero.MaxHealth}";
            HealStatusColor = Brushes.Green;
        
            Logs.Add($"{SelectedHero.Name} a reçu un soin {healTypeName.ToLower()}. PV: {SelectedHero.Health}/{SelectedHero.MaxHealth}");
        
            // Rafraîchir l'affichage
            RefreshResourceBindings();
            OnPropertyChanged(nameof(SelectedHeroStats));
            OnPropertyChanged(nameof(SelectedHero)); // AJOUTER CETTE LIGNE
            RefreshHeroes(); // AJOUTER CETTE LIGNE pour rafraîchir la liste complète
        }
        else
        {
            HealStatusMessage = "Pas assez de ressources pour ce soin.";
            HealStatusColor = Brushes.Red;
        }
    }

    private void Restart()
    {
        // Create a brand new engine
        _engine = new GameEngine();
        _merchant = new MerchantService(_engine.Guild);
        
        // Clear and repopulate all collections
        Heroes.Clear();
        foreach (var hero in _engine.Guild.Heroes)
        {
            Heroes.Add(hero);
        }

        Missions.Clear();
        foreach (var mission in _engine.Missions)
        {
            Missions.Add(mission);
        }

        RareItems.Clear();
        foreach (var item in _engine.Guild.RareItems)
        {
            RareItems.Add(item);
        }

        Offers.Clear();
        foreach (var offer in _merchant.GetDailyOffers())
        {
            Offers.Add(offer);
        }

        PendingAssignments.Clear();
        Logs.Clear();
        DefeatMessage = string.Empty;
        SelectedHero = null;
        _lastPhase = PhaseType.Evening;
        
        Logs.Add("===== NOUVELLE PARTIE =====");
        OnPropertyChanged(nameof(Money));
        OnPropertyChanged(nameof(Food));
        OnPropertyChanged(nameof(Medicine));
        OnPropertyChanged(nameof(Equipment));
        OnPropertyChanged(nameof(Debt));
        OnPropertyChanged(nameof(Day));
        RunPhase(PhaseType.Morning);
    }
}