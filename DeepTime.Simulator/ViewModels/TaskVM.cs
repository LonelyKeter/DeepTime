using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Windows;

using DeepTime.Lib;

using DeepTime.Simulator.Model;

namespace DeepTime.Simulator.ViewModels;

internal class TaskVM: DependencyObject
{
    public ObservableCollection<Task> Tasks { get; } = new();
    public TaskGeneratorVM TaskGeneratorVM { get; private init; } = new();

    #region Dependency props
    public Task SelectedTask
    {
        get { return (Task)GetValue(SelectedTaskProperty); }
        set { SetValue(SelectedTaskProperty, value); }
    }
    public static readonly DependencyProperty SelectedTaskProperty =
        DependencyProperty.Register("SelectedTask", typeof(Task), typeof(MainVM), new PropertyMetadata(null));
    

    #region Do task
    public int SpendMinutes
    {
        get { return (int)GetValue(SpendMinutesProperty); }
        set { SetValue(SpendMinutesProperty, value); }
    }
    public static readonly DependencyProperty SpendMinutesProperty =
        DependencyProperty.Register("SpendMinutes", typeof(int), typeof(MainVM), new PropertyMetadata(0));

    public int TimeLeft
    {
        get { return (int)GetValue(TimeLeftProperty); }
        set { SetValue(TimeLeftProperty, value); }
    }
    public static readonly DependencyProperty TimeLeftProperty =
        DependencyProperty.Register("TimeLeft", typeof(int), typeof(MainVM), new PropertyMetadata(0));
    #endregion

    #region Add task
    public string NewTaskTitle {get; set;} = string.Empty;
    public Lib.Data.Priority NewTaskPriority { get; set; } = Lib.Data.Priority.Medium;
    public Lib.Data.Priority[] PriorityVariants { get; } = Enum.GetValues<Lib.Data.Priority>();

    public Lib.Data.Attractiveness NewTaskAttractiveness { get; set; } = Lib.Data.Attractiveness.Medium;
    public Lib.Data.Attractiveness[] AttractivenessVariants { get; } = Enum.GetValues<Lib.Data.Attractiveness>();

    public int NewTaskMinuteEstimate
    {
        get { return (int)GetValue(NewTaskMinuteEstimateProperty); }
        set { SetValue(NewTaskMinuteEstimateProperty, value); }
    }
    public static readonly DependencyProperty NewTaskMinuteEstimateProperty =
        DependencyProperty.Register("NewTaskMinuteEstimate", typeof(int), typeof(MainVM), new PropertyMetadata(0));
    #endregion
#endregion

    public Task GetInputTask(int id)
    {
        var inner = new Lib.Data.Task(
            id,
            NewTaskAttractiveness,
            NewTaskPriority,
            NewTaskMinuteEstimate,
            0,
            false
        );

        return new Task(NewTaskTitle, inner);
    }    

    
}
