using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
namespace DeepTime.Simulator.Views;

using CommunityToolkit.Mvvm.DependencyInjection;

using Simulator.ViewModels;


/// <summary>
/// Логика взаимодействия для DoTaskPanel.xaml
/// </summary>
public partial class DoTaskPanel : UserControl
{
    public DoTaskPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<DoTaskVM>();
    }

    private void TaskListItem_SourceUpdated(object sender, DataTransferEventArgs e)
    {
        MessageBox.Show("Selected task updated");
    }
}
