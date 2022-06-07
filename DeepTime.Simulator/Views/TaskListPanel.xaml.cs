using System.Windows.Controls;

namespace DeepTime.Simulator.Views;
using CommunityToolkit.Mvvm.DependencyInjection;

using Simulator.ViewModels;


/// <summary>
/// Логика взаимодействия для TaskListPanel.xaml
/// </summary>
public partial class TaskListPanel : UserControl
{
    public TaskListPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<TaskListVM>();
    }

    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var vm = DataContext as TaskListVM;
        (sender as ListBox).ScrollIntoView(vm.SelectedTask);
    }
}
