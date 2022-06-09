namespace DeepTime.Advisor;

using DeepTime.Advisor.Data;
using DeepTime.Advisor.Statistics;

using DeepTime.RL;

using static DeepTime.Advisor.Data.Types;

public class Advisor<TTask> where TTask : ITask
{
    private readonly IScheduleSource _scheduleSource;
    private readonly AdvisorConfig _config;

    public double TotalReward { get; private set; }
    private ITaskManager<TTask> _taskManager;

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

    public Advisor(
        IAgent agent, 
        IScheduleSource scheduleSource, 
        AdvisorConfig config)
    {
        _agent = agent;
        _scheduleSource = scheduleSource;
        _config = config;
    }

    public IEnumerable<TTask>? GetAdvice()
    {
        var stateVec = CollectCurrentStateVec();
        _agent.SetNext(stateVec, 0.0);
        return GetAdviceFromUpdatedState(stateVec);        
    }

    private IEnumerable<TTask>? GetAdviceFromUpdatedState(double[] stateVec)
    {
        var action = AdviceEnumerator.GetValue(_agent.Eval());

        //CARE: Can loop if agent wont advise rest or valid task list
        while (!action.Rest && !_taskManager.GetUndone(action.Priority, action.Attractiveness).Any())
        {
            action = GetNextAction(stateVec, -1.0f);
        }

        return !action.Rest ? _taskManager.GetUndone(action.Priority, action.Attractiveness) : null;
    }

    public IEnumerable<TTask>? SubmitComplition(Priority priority)
    {
        var stateVec = CollectCurrentStateVec();
        var reward = _config.ComplitionRewards[priority.AsIndex()];
        TotalReward += reward;

        return GetAdviceFromUpdatedState(stateVec);
    }

    public void StartDay<T>(T taskManager) where T : ITaskManager<TTask>
    {
        _taskManager = taskManager;
        TotalReward = 0;

        Agent.StartEpisode(CollectCurrentStateVec());
    } 
        

    public StatisticsEntry FinishDay()
    {
        var vec = CollectCurrentStateVec();
        var progress = CountTasks(_taskManager);
        var penalty = CalculateEpisodePenalty(progress);

        TotalReward += penalty;

        Agent.EndEpisode(vec, penalty);

        var entry = new StatisticsEntry(progress, TotalReward);
        return entry;
    }

    private Advice GetNextAction(double[] state, double reward)
    {
        Agent.SetNext(state, reward);
        return AdviceEnumerator.GetValue(Agent.Eval());
    }

    private State CollectCurrentState()
    {
        var (todo, done) = WorkloadContext.GetTodoAndDone<ITaskManager<TTask>, TTask>(_taskManager);
        var scheduleContext = _scheduleSource.GetCurrent();

        return new State(todo, done, scheduleContext);
    }

    private double[] CollectCurrentStateVec() => StateConverter.ToVector(CollectCurrentState());

    private double CalculateEpisodePenalty(TaskEntry[] tasks) 
    {
        var penalty = 0.0;

        foreach (var pr in PriorityValues)
        {
            var (done, overall) = tasks[pr.AsIndex()];

            penalty += (overall - done) * _config.FailurePenalties[pr.AsIndex()];
        }

        return penalty;
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