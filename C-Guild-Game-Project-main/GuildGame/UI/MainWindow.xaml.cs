using System.Collections;
using System.Linq;
using System.Windows;
using GuildGame.Domain.Models;
using GuildGame.UI.ViewModels;

namespace GuildGame.UI;

public partial class MainWindow : Window
{
    private GameViewModel ViewModel => (GameViewModel)DataContext;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Assign_Click(object sender, RoutedEventArgs e)
    {
        if (MissionList.SelectedItem is not Mission mission) return;
        var selectedHeroes = HeroList.SelectedItems.Cast<Hero>().ToList();
        if (!selectedHeroes.Any())
        {
            MessageBox.Show("Sélectionnez au moins un héros dans la liste.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        ViewModel.AssignMission(mission, selectedHeroes);
    }
}
