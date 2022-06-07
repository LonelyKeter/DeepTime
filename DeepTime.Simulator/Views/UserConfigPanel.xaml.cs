using System.Windows.Controls;

namespace DeepTime.Simulator.Views;
using CommunityToolkit.Mvvm.DependencyInjection;

using Simulator.ViewModels;


/// <summary>
/// Логика взаимодействия для UserConfigPanel.xaml
/// </summary>
public partial class UserConfigPanel : UserControl
{
    public UserConfigPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<UserConfigVM>();
    }
}
