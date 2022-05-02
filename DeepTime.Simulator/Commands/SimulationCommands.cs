using System.Windows.Controls;
using System.Windows.Input;

namespace DeepTime.Simulator.Commands;

public static class SimulationCommands
{
    public static readonly RoutedCommand Rest = new("Rest", typeof(Control));
    public static readonly RoutedCommand DoTask = new("Do Task", typeof(Control));
    public static readonly RoutedCommand FinishTask = new("Finish Task", typeof(Control));
    public static readonly RoutedCommand StartDay = new("Start Day", typeof(Control));
    public static readonly RoutedCommand FinishDay = new("Finish Day", typeof(Control));
    public static readonly RoutedCommand SimulateDay = new("Simulate Day", typeof(Control));
    public static readonly RoutedCommand SimulateDays = new("Simulate Days", typeof(Control));
}
