using DeepTime.Lib.Data;

using Task = DeepTime.Lib.Data.Task;
using static DeepTime.Lib.Data.Types;
namespace DeepTime.Lib;

public class Advisor<TAgent, TScheduleSource>
    where TAgent : IAgent<State, Advice>
    where TScheduleSource : IScheduleSource
{
    private readonly TScheduleSource _scheduleSource;
    private readonly AdvisorConfig _config;
    public TAgent Agent { get; set; }

    public Advisor(TAgent agent, TScheduleSource scheduleSource, AdvisorConfig config)
    {
        Agent = agent;
        _scheduleSource = scheduleSource;
        _config = config;
    }

    public IEnumerable<Task>? GetAdvice<T>(T tasks) where T: ITaskManager
    {
        var state = CollectCurrentState(tasks);
        var action = GetNextAction(state, 0.0f);

        //CARE: Can loop if agent wont advise rest or valid task list
        while (!action.Rest && !tasks.GetUndone(action.Priority, action.Attractiveness).Any())
        {
            action = GetNextAction(state, -1.0f);
        }

        return !action.Rest ? tasks.GetUndone(action.Priority, action.Attractiveness) : null;
    }

    public void StartDay<T>(T tasks) where T : ITaskManager
        => Agent.StartEpisode(CollectCurrentState(tasks));

    public void FinishDay<T>(T tasks) where T : ITaskManager
        => Agent.EndEpisode(CollectCurrentState(tasks), CalculateEpisodeReward(tasks));

    private Advice GetNextAction(State state, double reward)
    {
        Agent.SetNext(state, reward);
        return Agent.Eval();
    }

    private State CollectCurrentState<T>(T tasks) where T : ITaskManager
    {
        var todo = WorkloadContext.GetTodo(tasks);
        var done = WorkloadContext.GetDone(tasks);

        var scheduleContext = _scheduleSource.GetCurrent();

        return new State(todo, done, scheduleContext);
    }    

    private double CalculateEpisodeReward<T>(T tasks) where T : ITaskManager
    {
        return tasks.Select(task => task.Done ? 
            _config.ComplitionRewards[task.Priority.AsIndex()] : 
            _config.FailurePenalties[task.Priority.AsIndex()]
        ).Sum();
    }
}

public record struct AdvisorConfig(double[] ComplitionRewards, double[] FailurePenalties)
{
    public static readonly AdvisorConfig Default = new(
        new double[] { 1.0f, 2.0f, 3.0f, 4.0f, 5.0f },
        new double[] { -2.0f, -4.0f, -6.0f, -8.0f, -10.0f }
    );
}