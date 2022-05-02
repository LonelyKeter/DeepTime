using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepTime.Lib.Data;

using Task = DeepTime.Lib.Data.Task;

namespace DeepTime.Simulation;

public class User : IUser
{
    private readonly Random _random = new();
    private UserState _state = InitialUserState;

    public UserConfig Config { get; init; }
    public UserState State => _state;

    public bool CanWork => CanWorkInState(_state);
    public bool WantsToWork => WantsToWorkInState(_state);

    public User(UserConfig config)
    {
        Config = config;
    }

    public UserFeedback? GetFeedback<P, T>(P? propositions, T tasks)
        where P : IEnumerable<Task>
        where T : IEnumerable<Task>
    {
        if (!CanWork) return null;

        if (propositions is not null && ChooseTaskFromPropositions(propositions, out var feedback))
            return feedback;

        return WantsToWork ? ChooseTaskIndependently(tasks) : null;
    }

    public void RestFor(int minutes)
    {
        if (!_state.IsResting)
        {
            _state.LastTaskId = null;
            _state.MinutesRested = minutes;
            _state.MinutesWorkedOnLastTask = 0;
            _state.MinutesWorkedContiniously = 0;
        }
        else
        {
            _state.MinutesRested += minutes;
        }
    }

    public void DoTask(UserFeedback feedback)
    {
        var (taskId, minutesSpent, _, _) = feedback;

        _state.MinutesWorked += minutesSpent;

        if (_state.IsResting)
        {
            _state.LastTaskId = taskId;
            _state.MinutesRested = 0;
        }
        else
        {
            if (taskId == _state.LastTaskId.Value)
                _state.MinutesWorkedOnLastTask += minutesSpent;
            else
                _state.MinutesWorkedOnLastTask = minutesSpent;

            _state.MinutesWorkedContiniously += minutesSpent;
        }
    }

    public void StartDay(IEnumerable<Task> tasks)
    {
        _state = InitialUserState;
    }

    private UserFeedback? ChooseTaskIndependently<T>(T tasks)
        where T : IEnumerable<Task>
    {
        return Config.Strategy switch
        {
            UserStrategy.AttractiveFirst => ChooseTaskAttractiveFirst(tasks),
            UserStrategy.PriorFirst => ChooseTaskPriorFirst(tasks),
            _ => throw new InvalidOperationException("Invalid user strategy.")
        };
    }


    private UserFeedback? ChooseTaskAttractiveFirst<T>(T tasks)
        where T : IEnumerable<Task>
    {
        Task? max = null;
        var maxAttractiveness = Attractiveness.VeryLow;

        foreach (var task in tasks.Where(task => !task.Done))
        {
            if (task.Attractiveness >= maxAttractiveness)
            {
                maxAttractiveness = task.Attractiveness;
                max = task;
            }
        }

        return max.HasValue ? PrepareFeedback(max.Value) : null;
    }

    private UserFeedback? ChooseTaskPriorFirst<T>(T tasks)
        where T : IEnumerable<Task>
    {
        Task? max = null;
        var maxPriority = Priority.VeryLow;

        foreach (var task in tasks.Where(task => !task.Done))
        {
            if (task.Priority >= maxPriority)
            {
                maxPriority = task.Priority;
                max = task;
            }
        }

        return max.HasValue ? PrepareFeedback(max.Value) : null;
    }

    private bool ChooseTaskFromPropositions<P>(P propositions, out UserFeedback? feedback)
        where P : IEnumerable<Task>
    {
        var choice = propositions
            .Cast<Task?>()
            .FirstOrDefault(prop => IsPropositonAcceptable(prop.Value));

        if (choice.HasValue)
        {
            feedback = PrepareFeedback(choice.Value);
            return true;
        }
        else
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

        var estimateIsCorrect = EstimateIsCorrect();

        if (estimateIsCorrect && task.SupposedlyDone || !estimateIsCorrect && task.DoneWorkPercentage >= 85)
        {
            return UserFeedback.FinishTask(task.Id, minutesSpent);
        }
        else
        {
            return UserFeedback.DoTask(task.Id, minutesSpent, estimateIsCorrect ? null : task.MinutesEstimate / 5);
        }
    }

    private bool CanWorkInState(UserState state)
        => state.MinutesWorked < Config.MaxWorkMinutes
        && state.MinutesWorkedContiniously < Config.MaxContiniousWorkMinutes
        || state.IsResting
        && state.MinutesRested >= Config.MinRest;

    public bool WantsToWorkInState(UserState state)
        => (double)state.MinutesWorkedContiniously / Config.MaxContiniousWorkMinutes <= Config.Initiativeness;

    private bool IsPropositonAcceptable(Task prop)
    {

        if (PriorityAcceptable(prop.Priority) || AttractivenessAcceptable(prop.Attractiveness))
            return _state.IsResting || prop.Id != _state.LastTaskId.Value || State.MinutesWorkedOnLastTask < Config.MaxMinutesOnOneTask;
        else
            return Config.Initiativeness > _random.NextDouble();

    }

    private bool EstimateIsCorrect()
        => _random.NextDouble() <= Config.EstimateAccuracy;

    private static readonly UserState InitialUserState = new()
    {
        LastTaskId = null,
        MinutesRested = 60,
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
    public static readonly UserConfig Default = new(
            MaxWorkMinutes: 60 * 8,
            Initiativeness: 0.3,
            MaxContiniousWorkMinutes: 150,
            MaxMinutesOnOneTask: 90,
            MinRest: 20,
            EstimateAccuracy: 0.8,
            Strategy: UserStrategy.PriorFirst
    );
}

public struct UserState
{
    public int MinutesWorked { get; internal set; }
    public int MinutesWorkedOnLastTask { get; internal set; }
    public int MinutesWorkedContiniously { get; internal set; }
    public int MinutesRested { get; internal set; }
    public int? LastTaskId { get; internal set; }

    public bool IsResting
        => !LastTaskId.HasValue;
}