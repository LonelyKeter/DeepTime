using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepTime.Lib.Data;

using Task = DeepTime.Lib.Data.Task;

namespace DeepTime.Simulation;

public class User : IUser
{
    private readonly Dictionary<int, Task> _tasks = new();
    private readonly Random _random = new();
    private UserState _state = CreateInitialUserState(TimeOnly.MaxValue);

    public UserConfig Config { get; init; }
    public UserState State => _state;
    public IReadOnlyDictionary<int, Task> Tasks => _tasks;


    public bool CanWork
        => _state.MinutesWorked < Config.MaxWorkMinutes && _state.MinutesWorkedContiniously < Config.MaxContiniousWorkMinutes;
    public bool WantsToWork
        => (double)State.MinutesWorkedContiniously / Config.MaxContiniousWorkMinutes > Config.Initiativeness;
    

    public User(UserConfig config)
    {
        Config = config;
    }

    public UserFeedback? GetFeedback(IEnumerable<Task>? propositions, TimeOnly time)
    {
        Update(time);

        var decision = !CanWork ? null :
            ChooseTaskFromPropositions(propositions, out var feedback) ? feedback :
            WantsToWork ? ChooseTaskIndependently() : null;

        Update(decision);

        return decision;
    }

    public void StartDay(TimeOnly time, IEnumerable<Task> tasks)
    {
        _state = CreateInitialUserState(time);
        InitTaskList(tasks);        
    }

    private UserFeedback? ChooseTaskIndependently() => Config.Strategy switch
    {
        UserStrategy.AttractiveFirst => ChooseTaskAttractiveFirst(),
        UserStrategy.PriorFirst => ChooseTaskPriorFirst(),
        _ => throw new InvalidOperationException("Invalid user strategy.")
    };


    private UserFeedback? ChooseTaskAttractiveFirst()
    {
        var task = _tasks.Values.Where(task => !task.Done).MaxBy(task => (int)task.Attractiveness);

        return PrepareFeedback(task);
    }

    private UserFeedback? ChooseTaskPriorFirst()
    {
        var task = _tasks.Values.Where(task => !task.Done).MaxBy(task => (int)task.Priority);

        return PrepareFeedback(task);
    }

    private bool ChooseTaskFromPropositions(IEnumerable<Task>? propositions, out UserFeedback? feedback)
    {
        if (propositions is null)
        {
            feedback = null;
            return false;
        }

        try
        {
            var choice = propositions.First(prop =>
            {
                if (PriorityAcceptable(prop.Priority) || AttractivenessAcceptable(prop.Attractiveness))
                    return _state.IsResting || prop.Id != _state.LastTaskId.Value || State.MinutesWorkedOnLastTask < Config.MaxMinutesOnOneTask;
                else
                    return Config.Initiativeness > _random.NextDouble();
            });

            feedback = PrepareFeedback(choice);
            return true;
        }
        catch (InvalidOperationException)
        {
            feedback = null;
            return false;
        }
    }

    private bool PriorityAcceptable(Priority priority)
        => priority >= Priority.Medium && Config.Strategy == UserStrategy.PriorFirst;

    private bool AttractivenessAcceptable(Attractiveness attractiveness)
        => attractiveness >= Attractiveness.Medium && Config.Strategy == UserStrategy.AttractiveFirst;

    private UserFeedback PrepareFeedback(Task task)
    {
        var minutesSpent = Math.Min(
            task.Attractiveness >= Attractiveness.High ? 30 : 15,
            task.LeftEstimate
        );

        var newEstimate = task.LeftEstimate;
        var done = false;
        if (_random.NextDouble() > Config.EstimateAccuracy)
        {
            if (task.LeftEstimate <= task.MinutesEstimate / 5)
            {
                done = true;
            }
            else
            {
                newEstimate += 10;
            }
        }

        return new(task.Id, minutesSpent, done, newEstimate);
    }


    private void Update(UserFeedback? userFeedback)
    {
        UpdateTasks(userFeedback);
        UpdateState(userFeedback);
    }

    private void Update(TimeOnly time)
    {
        //TODO: Time cycle check policy
        if (_state.CurrentTime >= time) return;

        _state.CurrentTime = time;

        if (_state.IsResting)
            _state.MinutesRested += (time - _state.RestStarted).Minutes;
    }

    private void UpdateTasks(UserFeedback? feedback)
    {
        if (feedback is null) return;

        var value = feedback.Value;
        var task = _tasks[value.TaskId];

        _tasks[value.TaskId] = task with
        {
            MinutesSpent = task.MinutesSpent + value.MinutesSpent,
            MinutesEstimate = task.MinutesEstimate + value.NewEstimate,
            Done = task.Done,
        };
    }

    private void UpdateState(UserFeedback? feedback)
    {
        //If user chose to stop doing tasks
        if (feedback is null)
        {
            if (!_state.IsResting)
            {
                _state.LastTaskId = null;
                _state.MinutesRested = 0;
                _state.RestStarted = _state.CurrentTime;
            }
        }
        else
        {
            var (taskId, minutesSpent, _, _) = feedback.Value;

            if (_state.IsResting)
            {
                _state.LastTaskId = taskId;
            }
            else
            {
                if (taskId == _state.LastTaskId.Value)
                    _state.MinutesWorkedOnLastTask += minutesSpent;

                _state.MinutesWorkedContiniously += minutesSpent;
            }

            _state.MinutesWorked += minutesSpent;
        }
    }

    private void InitTaskList(IEnumerable<Task> tasks)
    {
        _tasks.Clear();
        
        foreach (var task in tasks)
        {
            _tasks.Add(task.Id, task);
        }
    }

    private static UserState CreateInitialUserState(TimeOnly time) => new()
    {
        CurrentTime = time,
        RestStarted = time,
        LastTaskId = null,
        MinutesRested = int.MaxValue,
        MinutesWorked = 0,
        MinutesWorkedContiniously = 0,
        MinutesWorkedOnLastTask = 0,
    };
}

public enum UserStrategy
{
    AttractiveFirst,
    PriorFirst
}

public record UserConfig(
    int MaxWorkMinutes,
    double Initiativeness,
    int MaxContiniousWorkMinutes,
    int MaxMinutesOnOneTask,
    int MinRest,
    double EstimateAccuracy,
    UserStrategy Strategy
)
{
    public static readonly UserConfig Default = new UserConfig(
            60 * 8,
            0.7,
            150,
            90,
            20,
            0.8,
            UserStrategy.PriorFirst
        );
}

public struct UserState {
    public int MinutesWorked { get; internal set; }
    public int MinutesWorkedOnLastTask { get; internal set; }
    public int MinutesWorkedContiniously { get; internal set; }
    public int MinutesRested { get; internal set; }
    public int? LastTaskId { get; internal set; }
    public TimeOnly RestStarted { get; internal set; }
    public TimeOnly CurrentTime { get; internal set; }

    public bool IsResting
        => !LastTaskId.HasValue;
}