namespace DeepTime.Advisor;

using DeepTime.Advisor.Data;
using DeepTime.Advisor.Statistics;

using DeepTime.RL;

using static DeepTime.Advisor.Data.Types;

public class Advisor<TTask> where TTask : ITask
{
    private readonly IScheduleSource _scheduleSource;
    private readonly AdvisorConfig _config;

    public static readonly int StateVecLength = StateConverter.InputSize;
    public static readonly int ActionCount = AdviceEnumerator.EnumCount;

    private IAgent _agent;
    public IAgent Agent { 
        get => _agent;
        set
        {
            _agent = value;
        }
    }
    public IStatistics Statistics { get; } = new Statistics.Statistics();

    public Advisor(
        IAgent agent, 
        IScheduleSource scheduleSource, 
        AdvisorConfig config)
    {
        _agent = agent;
        _scheduleSource = scheduleSource;
        _config = config;
    }

    public IEnumerable<TTask>? GetAdvice<T>(T tasks) where T: ITaskManager<TTask>
    {
        var state = CollectCurrentState(tasks);
        var stateVec = StateConverter.ToVector(state);
        var action = GetNextAction(stateVec, 0.0f);

        //CARE: Can loop if agent wont advise rest or valid task list
        while (!action.Rest && !tasks.GetUndone(action.Priority, action.Attractiveness).Any())
        {
            action = GetNextAction(stateVec, -1.0f);
        }

        return !action.Rest ? tasks.GetUndone(action.Priority, action.Attractiveness) : null;
    }

    public void StartDay<T>(T tasks) where T : ITaskManager<TTask>
        => Agent.StartEpisode(
                StateConverter.ToVector(
                    CollectCurrentState(tasks)
                )
           );

    public StatisticsEntry FinishDay<T>(T tasks) where T : ITaskManager<TTask>
    {
        var state = CollectCurrentState(tasks);
        var vec = StateConverter.ToVector(state);
        var progress = CountTasks(tasks);
        var reward = CalculateEpisodeReward(progress);

        Agent.EndEpisode(vec, reward);

        var entry = new StatisticsEntry(progress, reward);
        Statistics.Submit(entry);
        return entry;
    }

    private Advice GetNextAction(double[] state, double reward)
    {
        Agent.SetNext(state, reward);
        return AdviceEnumerator.GetValue(Agent.Eval());
    }

    private State CollectCurrentState<T>(T tasks) where T : ITaskManager<TTask>
    {
        var (todo, done) = WorkloadContext.GetTodoAndDone<T, TTask>(tasks);
        var scheduleContext = _scheduleSource.GetCurrent();

        return new State(todo, done, scheduleContext);
    }    

    private double CalculateEpisodeReward(TaskEntry[] tasks) 
    {
        var reward = 0.0;

        foreach (var pr in PriorityValues)
        {
            var (done, overall) = tasks[pr.AsIndex()];

            reward += done * _config.ComplitionRewards[pr.AsIndex()];
            reward += (overall - done) * _config.FailurePenalties[pr.AsIndex()];
        }

        return reward;
    }    

    private static TaskEntry[] CountTasks<T>(T tasks) where T : ITaskManager<TTask>
    {
        var result = new TaskEntry[PriorityCount];

        foreach (var task in tasks)
        {
            result[task.Priority.AsIndex()].Done += task.Done ? 1 : 0;
            result[task.Priority.AsIndex()].Todo++;
        }

        return result;
    }
}

public record struct AdvisorConfig(double[] ComplitionRewards, double[] FailurePenalties)
{
    public static readonly AdvisorConfig Default = new(
        new double[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f },
        new double[] { -2.0f, -4.0f, -6.0f, -8.0f, -10.0f }
    );
}