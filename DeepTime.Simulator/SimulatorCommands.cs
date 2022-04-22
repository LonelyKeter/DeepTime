using System.Windows.Controls;
using System.Windows.Input;

namespace DeepTime.Simulator;

public static class TestCommands
{
    public static readonly RoutedCommand Rest = new("Rest", typeof(Control));
    public static readonly RoutedCommand DoTask = new("Do Task", typeof(Control));
    public static readonly RoutedCommand FinishTask = new("Finish Task", typeof(Control));
    public static readonly RoutedCommand AddTask = new("Add Task", typeof(Control));
    public static readonly RoutedCommand StartDay = new("Start Day", typeof(Control));
    public static readonly RoutedCommand FinishDay = new("Finish Day", typeof(Control));
}
