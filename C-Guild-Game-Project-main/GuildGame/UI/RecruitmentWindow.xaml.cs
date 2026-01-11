using System;
using System.Windows;
using GuildGame.UI.ViewModels;

namespace GuildGame.UI;

public partial class RecruitmentWindow : Window
{
    private RecruitmentViewModel? _viewModel;

    public RecruitmentWindow(RecruitmentViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = viewModel;
        _viewModel.HeroRecruited += (s, e) => 
        {
            MessageBox.Show($"Héros recruté! Il arrivera demain.", "Recrutement");
        };
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}