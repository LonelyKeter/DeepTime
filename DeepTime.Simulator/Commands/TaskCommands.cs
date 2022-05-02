using System.Windows.Controls;
using System.Windows.Input;

namespace DeepTime.Simulator.Commands;

public class TaskCommands
{
    public static readonly RoutedCommand AddTask = new("Add Task", typeof(Control));
    public static readonly RoutedCommand GenerateTasks = new("Generate tasks", typeof(Control));
    public static readonly RoutedCommand GenerateDay = new("Generate day", typeof(Control));
    public static readonly RoutedCommand ClearTasks = new("Clear tasks", typeof(Control));
    public static readonly RoutedCommand DeleteTask = new("Delete task", typeof(Control));
}
