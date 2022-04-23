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

    public UserConfig Config { get; init; }
    public UserState State { get; init; }
    public IReadOnlyDictionary<int, Task> Tasks => _tasks;


    public bool CanWork => ;
    public bool WantsToWork
        => (double)State.MinutesWorkedContiniously / Config.MaxContiniousWorkMinutes > Config.Initiativeness;

    public UserFeedback? GetFeedback(IEnumerable<Task>? propositions, TimeOnly time)
    {
        UpdateTime(time);

        var decision = !CanWork ? null :
            ChooseTaskFromPropositions(propositions, out var feedback) ? feedback :
            WantsToWork ? ChooseTaskIndependently() : null;

        UpdateWithFeedBack(decision);

        return decision;
    }

    public List<Task> InitDay()
    {
        throw new NotImplementedException();
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

        return DoTask(task);
    }

    private UserFeedback? ChooseTaskPriorFirst()
    {
        var task = _tasks.Values.Where(task => !task.Done).MaxBy(task => (int)task.Priority);

        return DoTask(task);
    }

    private void UpdateTime(TimeOnly time)
    {
        if (time == State.CurrentTime) return;
    }

    private void UpdateWithFeedBack(UserFeedback? feedback)
    {
        if (feedback is null) return;
        var value = feedback.Value;

        if (value.Done)
        {
            _tasks[value.TaskId] = _tasks[value.TaskId] with { Done = true };

        }
        else
        {
            var task = _tasks[value.TaskId];
            _tasks[value.TaskId] = task with
            {
                MinutesSpent = task.MinutesSpent + value.MinutesSpent,
                MinutesEstimate = task.MinutesEstimate + value.NewEstimate
            };
        }
    }

    private bool ChooseTaskFromPropositions(IEnumerable<Task>? propositions, out UserFeedback? feedback)
    {
        if (propositions is null)
        {
            feedback = null;
            return false;
        }

        Task? choice;
        try
        {
            choice = propositions.First(prop =>
            {
                if (PriorityAcceptable(prop.Priority) || AttractivenessAcceptable(prop.Attractiveness))
                    return !State.LastTask.HasValue || prop.Id != State.LastTask.Value.Id || State.MinutesWorkedOnLastTask < Config.MaxMinutesOnOneTask;
                else
                    return Config.Initiativeness > _random.NextDouble();
            });
        }
        catch (InvalidOperationException)
        {
            choice = null;
        }

        if (choice is null)
        {
            feedback = null;
            return false;
        }
        else
        {
            feedback = DoTask(choice.Value);
            return true;
        }
    }

    private bool PriorityAcceptable(Priority priority)
        => priority >= Priority.Medium && Config.Strategy == UserStrategy.PriorFirst;

    private bool AttractivenessAcceptable(Attractiveness attractiveness)
        => attractiveness >= Attractiveness.Medium && Config.Strategy == UserStrategy.AttractiveFirst;

    private UserFeedback DoTask(Task task)
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
}

public enum UserStrategy
{
    AttractiveFirst,
    PriorFirst
}

public record UserConfig(
    int MaxWorkMinutes,
    double[] AttractivenessPenalty,
    double Initiativeness,
    int MaxContiniousWorkMinutes,
    int MaxMinutesOnOneTask,
    int RestFactor,
    int MinRest,
    double EstimateAccuracy,
    UserStrategy Strategy
);

public record UserState(
    int MinutesWorked,
    int MinutesWorkedOnLastTask,
    int MinutesWorkedContiniously,
    int MinutesRested,
    Task? LastTask,
    TimeOnly CurrentTime
    );