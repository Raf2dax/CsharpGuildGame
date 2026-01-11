using System.Windows;
using GuildGame.UI.ViewModels;

namespace GuildGame.UI;

public partial class MerchantWindow : Window
{
    public MerchantWindow(GameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
