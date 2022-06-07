namespace DeepTime.Simulator.Views;

using CommunityToolkit.Mvvm.DependencyInjection;

using Simulator.ViewModels;

using System.Windows.Controls;


/// <summary>
/// Логика взаимодействия для AddTaskPanel.xaml
/// </summary>
public partial class AddTaskPanel : UserControl
{
    public AddTaskPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AddTaskVM>();
    }
}
