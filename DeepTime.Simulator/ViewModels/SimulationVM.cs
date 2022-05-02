using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;

using DeepTime.Lib;
using DeepTime.Simulation;
using DeepTime.Simulator.Commands;
using DeepTime.Lib.Data;

using Agent = DeepTime.Lib.IAgent<DeepTime.Lib.Data.State, DeepTime.Lib.Data.Advice>;

using static DeepTime.Simulator.ViewModels.ViewModel;


namespace DeepTime.Simulator.ViewModels;

public class SimulationVM : DependencyObject
{
    private SimulationTool<Agent, User, TaskGenerator> _simulator;
    private readonly TaskGenerator _taskGenerator = new();

    private int _selectedTaskId => TaskVM.SelectedTask.Inner.Id;
    private bool _selectedTaskDone => TaskVM.SelectedTask.Inner.Done;

    public TaskVM TaskVM { get; }
    public UserVM UserVM { get; } = new();

    public DQNVM DQNVM { get; } = new();

    #region Dependency props 
    public ScheduleContext ScheduleContext
    {
        get => (ScheduleContext)GetValue(ScheduleContextProperty);
        set => SetValue(ScheduleContextProperty, value);
    }
    public static readonly DependencyProperty ScheduleContextProperty =
        DependencyProperty.Register(
            "ScheduleContext",
            typeof(ScheduleContext),
            typeof(SimulationVM),
            new PropertyMetadata()
        );

    public int MinutesSpent
    {
        get => (int)GetValue(MinutesSpentProperty);
        set => SetValue(MinutesSpentProperty, value);
    }
    public static readonly DependencyProperty MinutesSpentProperty =
        DependencyProperty.Register(
            "MinutesSpent",
            typeof(int),
            typeof(SimulationVM),
            new PropertyMetadata(0)
        );

    public int? NewEstimate
    {
        get => (int?)GetValue(NewEstimateProperty);
        set => SetValue(NewEstimateProperty, value);
    }
    public static readonly DependencyProperty NewEstimateProperty =
        DependencyProperty.Register(
            "NewEstimate",
            typeof(int?),
            typeof(SimulationVM),
            new PropertyMetadata(0)
        );   

    public bool Finished
    {
        get => (bool)GetValue(FinishedProperty);
        set => SetValue(FinishedProperty, value);
    }
    public static readonly DependencyProperty FinishedProperty =
        DependencyProperty.Register(
            "Finished",
            typeof(bool),
            typeof(SimulationVM),
            new PropertyMetadata(false)
        );
    #endregion

    public SimulationVM()
    {
        var config = CreateConfig(UserVM.User, DQNVM.Agent);
        _simulator = new(config);

        TaskVM = new(_simulator.TaskGenerator, _simulator.TaskManager);

        UpdateScheduleContext();
    }
    public IList CreateCommandBindings() => new[]
    {
        CreateCommandBinding(SimulationCommands.DoTask, DoTask, DoTaskCanExecute),
        CreateCommandBinding(SimulationCommands.Rest, Rest, RestCanExecute),
        CreateCommandBinding(SimulationCommands.StartDay, StartDay, StartDayCanExecute),
        CreateCommandBinding(SimulationCommands.FinishDay, FinishDay, FinishDayCanExecute),
    };

    public void DoTask()
    {
        if (!DoTaskCanExecute()) 
            throw new InvalidOperationException();

        if (Finished)
        {
            _simulator.FinishTask(_selectedTaskId, MinutesSpent);
        }
        else
        {
            _simulator.DoTask(_selectedTaskId, MinutesSpent, NewEstimate);
        }

        Update(_selectedTaskId);
    }
    private bool DoTaskCanExecute() 
        => _simulator.DayGoes && TaskVM.SelectedTask != null && !_selectedTaskDone;

    public void Rest()
    {
        _simulator.Rest();
        Update();
    }
    private bool RestCanExecute()
        => _simulator.DayGoes;

    public void StartDay()
    {
        _simulator.StartNextDay();
        TaskVM.UpdateTaskList();
        Update();
        TaskVM.TaskGenerationEnabled = true;
    }
    private bool StartDayCanExecute()
        => !_simulator.DayGoes;

    public void FinishDay()
    {
        _simulator.FinishDay();
        UpdateScheduleContext();
        TaskVM.TaskGenerationEnabled = false;
    }
    private bool FinishDayCanExecute()
        => _simulator.DayGoes;

    private void UpdateScheduleContext()
    {
        ScheduleContext = _simulator.ScheduleContext;
    }

    private void Update()
    {
        UpdateScheduleContext();
        TaskVM.UpdateSuggestions(_simulator.LastAdvice);
        UserVM.UpdateState();
        UpdateUserFeedback();
    }

    private void Update(int taskId)
    {
        TaskVM.UpdateTask(_selectedTaskId);
        Update();
    }

    private void UpdateUserFeedback()
    {
        if (_simulator.CurrentFeedback.HasValue)
        {
            var feedback = _simulator.CurrentFeedback.Value;

            TaskVM.SetSelectedTask(feedback.TaskId);
            MinutesSpent = feedback.MinutesSpent;
            NewEstimate = feedback.NewEstimate;
            Finished = feedback.Done;
        }
        else
        {
            TaskVM.SelectedIndex = -1;
            MinutesSpent = 0;
            NewEstimate = null;
            Finished = false;
        }
    }

    private SimulationConfig<Agent, User, TaskGenerator> CreateConfig(User user, Agent agent)
        => new(
            user, 
            agent, 
            _taskGenerator, 
            new()
            {
                Start = new(8, 0),
                End = new(21, 0)
            }, AdvisorConfig.Default);
}
