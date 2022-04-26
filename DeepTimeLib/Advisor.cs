﻿using DeepTime.Lib.Data;

using Task = DeepTime.Lib.Data.Task;

namespace DeepTime.Lib;

public class Advisor<TAgent, TScheduleSource>
    where TAgent : IAgent<State, Advice>
    where TScheduleSource : IScheduleSource
{
    private readonly TAgent _agent;
    private readonly TScheduleSource _scheduleSource;
    private readonly AdvisorConfig _config;

    public List<Task> Tasks { get; } = new List<Task>();

    public IEnumerable<Task>? GetAdvice()
    {
        var state = CollectCurrentState();
        var (action, predicate) = GetNextActionWithPredicate(state);

        //CARE: Can loop if agent wont advise rest or valid task list
        while (!action.Rest || !Tasks.Any(predicate))
        {
            (action, predicate) = GetNextActionWithPredicate(state);
        }

        return !action.Rest ? Tasks.Where(predicate) : null;
    }

    public void SubmitProgress(int taskId, int minutesSpent, bool done = false, int newEstimate = 0)
    {
        var index = Tasks.FindIndex(task => task.Id == taskId);

        if (index == -1)
        {
            throw new ArgumentOutOfRangeException(nameof(taskId), "Invalid task id submitted.");
        }

        var taskUpdate = Tasks[index];

        if (taskUpdate.Done)
        {
            throw new InvalidOperationException($"Task {taskUpdate.Id} was already done.");
        }

        if (newEstimate < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(newEstimate), "New estimate should be greater or equal to zero.");
        }

        taskUpdate.MinutesSpent += minutesSpent;
        taskUpdate.Done = done;
        taskUpdate.MinutesEstimate = taskUpdate.MinutesSpent + newEstimate;

        Tasks[index] = taskUpdate;
    }

    public void StartDay()
        => _agent.StartEpisode(CollectCurrentState());

    public void FinishDay() 
        => _agent.EndEpisode(CollectCurrentState(), CalculateEpisodeReward());

    private (Advice, Func<Task, bool>) GetNextActionWithPredicate(State state)
    {
        var action = GetNextAction(state);

        return (action, GetTaskPredicate(action));
    }

    private static Func<Task, bool> GetTaskPredicate(Advice action) =>
        task => task.Priority == action.Priority && task.Attractiveness == action.Attractiveness && !task.Done;


    private Advice GetNextAction(State state)
    {
        _agent.SetNext(state, 0.0f);
        return _agent.Eval();
    }

    private State CollectCurrentState()
    {
        var todo = CollectTodoWorkloadContext(Tasks);
        var done = CollectDoneWorkloadContext(Tasks);

        var scheduleContext = _scheduleSource.GetCurrent();

        return new State(todo, done, scheduleContext);
    }

    private static WorkloadContext CollectDoneWorkloadContext<TTaskCollection>(TTaskCollection collection)
        where TTaskCollection : IEnumerable<Task>
    {
        var entryTable = new WorkloadContextEntry[5, 5];

        foreach (var task in collection)
        {
            entryTable[(int)task.Priority, (int)task.Attractiveness].MinutesEstimate +=
                task.Done ? task.MinutesSpent : task.MinutesEstimate - task.MinutesSpent;

            entryTable[(int)task.Priority, (int)task.Attractiveness].Count +=
                task.Done ? 1 : 0;
        }

        return new WorkloadContext(entryTable);
    }

    private static WorkloadContext CollectTodoWorkloadContext<TTaskCollection>(TTaskCollection collection)
        where TTaskCollection : IEnumerable<Task>
    {
        var entryTable = new WorkloadContextEntry[5, 5];

        foreach (var task in collection)
        {
            entryTable[(int)task.Priority, (int)task.Attractiveness].MinutesEstimate +=
                task.Done ? task.MinutesSpent : task.MinutesEstimate;
            entryTable[(int)task.Priority, (int)task.Attractiveness].Count += 1;
        }

        return new WorkloadContext(entryTable);
    }

    private float CalculateEpisodeReward() 
        => Tasks.Select(task => 
        task.Done ? _config.ComplitionRewards[(int)task.Priority] : _config.FailurePenalties[(int)task.Priority]
        ).Sum();
}

public record struct AdvisorConfig(float[] ComplitionRewards, float[] FailurePenalties)
{
    public static readonly AdvisorConfig Default = new(
        new float[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f },
        new float[] { -2.0f, -4.0f, -6.0f, -8.0f, -10.0f }
    );
}