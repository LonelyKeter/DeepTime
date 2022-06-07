using System.Windows.Controls;

namespace DeepTime.Simulator.Views;
using CommunityToolkit.Mvvm.DependencyInjection;

using Simulator.ViewModels;


/// <summary>
/// Логика взаимодействия для TaskGeneratorPanel.xaml
/// </summary>
public partial class TaskGeneratorPanel : UserControl
{
    public TaskGeneratorPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<TaskGeneratorVM>();
    }
}
