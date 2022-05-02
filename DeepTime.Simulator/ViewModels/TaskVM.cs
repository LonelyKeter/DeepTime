using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using DeepTime.Simulator.Model;
using DeepTime.Simulator.ViewModels;
using DeepTime.Simulator.Commands;

using DeepTime.Lib;

using DeepTime.Simulation;

using static DeepTime.Simulator.ViewModels.ViewModel;

namespace DeepTime.Simulator.ViewModels;

public class TaskVM: DependencyObject
{
    public TaskGenerator TaskGenerator { get; } 
    public TaskManager TaskManager { get; }
    public ObservableCollection<Task> Tasks { get; } = new();

    public TaskGeneratorVM TaskGeneratorVM { get; }

    public bool TaskGenerationEnabled { get; set; }
    
    public TaskVM(TaskGenerator generator, TaskManager manager)
    {
        TaskGenerator = generator;
        TaskGeneratorVM = new(TaskGenerator);

        TaskManager = manager;
    }

    #region Dependency props
    public Task SelectedTask
    {
        get { return (Task)GetValue(SelectedTaskProperty); }
        set { SetValue(SelectedTaskProperty, value); }
    }
    public static readonly DependencyProperty SelectedTaskProperty =
        DependencyProperty.Register("SelectedTask", typeof(Task), typeof(MainVM), new PropertyMetadata(null));

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }
    public static readonly DependencyProperty SelectedIndexProperty =
        DependencyProperty.Register(
            "SelectedIndex",
            typeof(int),
            typeof(TaskVM),
            new PropertyMetadata(-1)
        );


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

    public IList CreateCommandBindings() => new[]
    {
        CreateCommandBinding(TaskCommands.GenerateDay, GenerateDay, GenerateDayCanExecute),
        CreateCommandBinding(TaskCommands.GenerateTasks, GenerateTasks, GenerateTasksCanExecute),
        CreateCommandBinding(TaskCommands.AddTask, AddTask, AddTaskCanExecute),
        CreateCommandBinding(TaskCommands.ClearTasks, ClearTasks, ClearTasksCanExecute),
        CreateCommandBinding(TaskCommands.DeleteTask, DeleteTask, DeleteTaskCanExecute),
    };

    public void UpdateTaskList()
    {
        Tasks.Clear();

        foreach (var task in TaskManager)
        {
            Tasks.Add(new(string.Empty, task));
        }
    }

    public void UpdateTask(int id)
    {
        var task = TaskManager[id];
        Tasks.First(task => task.Inner.Id == id).Inner = task;
    }

    public void SetSelectedTask(int id)
    {
        var taskIndex = Tasks.TakeWhile(task => task.Inner.Id != id).Count();
        SelectedIndex = taskIndex;
    }

    public void UpdateSuggestions(IReadOnlyList<Lib.Data.Task> suggestedTasks)
    {
        foreach (var task in Tasks)
        {
            task.Proposed = suggestedTasks.Contains(task.Inner);
        }
    } 

    public void GenerateDay() 
    {
        ClearTasks();
        var tasks = TaskGenerator.GenDay().ToArray();
        AddTasks(tasks);
    }
    private bool GenerateDayCanExecute()
        => TaskGenerationEnabled && TaskGenerator.GeneratesAny();

    public void GenerateTasks()
    {
        var tasks = (TaskGenerator as ITaskGenerator).GenTasks(TaskGeneratorVM.TaskCount);
        AddTasks(tasks);
    }
    private bool GenerateTasksCanExecute()
        => TaskGenerationEnabled && TaskGenerator.GeneratesAny();

    public void DeleteTask()
    {
        var task = SelectedTask;
        TaskManager.Remove(task.Inner.Id);
        Tasks.Remove(task);
    }
    private bool DeleteTaskCanExecute()
        => SelectedTask is not null;

    public void ClearTasks()
    {
        TaskManager.Clear();
        Tasks.Clear();
        TaskGenerator.ResetCounter();
    }
    private bool ClearTasksCanExecute()
        => Tasks.Any();

    public void AddTask()
    {
        var task = TaskGenerator.GenTask(NewTaskPriority, NewTaskAttractiveness, NewTaskMinuteEstimate);        
        AddTask(task);
    }

    private bool AddTaskCanExecute()
        => TaskGenerationEnabled && NewTaskMinuteEstimate > 0;

    private void AddTask(Lib.Data.Task task) 
    {
        TaskManager.Add(task);
        Tasks.Add(new(NewTaskTitle, task));
    }

    private void AddTasks(IEnumerable<Lib.Data.Task> tasks)
    {
        foreach (var task in tasks)
        {
            AddTask(task);
        }
    }
}
