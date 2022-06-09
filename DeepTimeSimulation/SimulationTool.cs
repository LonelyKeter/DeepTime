namespace DeepTime.Simulation;

using DeepTime.Advisor;
using DeepTime.Advisor.Data;
using DeepTime.Advisor.Statistics;
using DeepTime.RL;

public class SimulationTool<TTask> where TTask : ITask
{
    private readonly Advisor<TTask> _advisor;
    private readonly IUser<TTask> _user;
    private readonly SimulatedScheduleSouce _scheduleSource;    
    public ITaskManager<TTask> TaskManager { get; }
    public ITaskGenerator<TTask> TaskGenerator { get; set; }


    public IAgent Agent
    {
        get => _advisor.Agent;
        set => _advisor.Agent = value;
    }

    private readonly List<TTask> _lastAdvice = new();
    public IReadOnlyList<TTask> LastAdvice => _lastAdvice;

    public UserFeedback? CurrentFeedback { get; private set; } = null;
    public ScheduleContext ScheduleContext => _scheduleSource.GetCurrent();

    public bool DayGoes { get; private set; } = false;
    public int MinQueryMinutes { get; set; } = 10;

    public SimulationTool(SimulationConfig<TTask> config)
    {
        _user = config.User;
        _scheduleSource = config.ScheduleSouce;
        _advisor = new(
            config.Agent, 
            config.ScheduleSouce, 
            config.AdvisorConfig);

        TaskGenerator = config.TaskGenerator;
        TaskManager = config.TaskManager;
    }

    public bool StepForward()
    {
        ApplyAction();

        CurrentFeedback = _user.GetFeedback(_lastAdvice, TaskManager);

        return !TaskManager.GetUndone().Any();
    }

    public (uint, StatisticsEntry) SimulateDay()
    {
        StartNextDay();
        while (!_scheduleSource.DayHasPassed())
        {
            if (StepForward()) break;
        }
        return FinishDay();
    }

    public IEnumerable<(uint, StatisticsEntry)> SimulateDays(int dayCount)
    {
        for (var i = 0; i < dayCount; i++)
        {
            yield return SimulateDay();
        }
    }

    public void DoTask(int id, int minutesSpent, int? newEstimate)
    {
        CurrentFeedback = UserFeedback.DoTask(id, minutesSpent, newEstimate);
        StepForward();
    }

    public void FinishTask(int id, int minutesSpent)
    {
        CurrentFeedback = UserFeedback.FinishTask(id, minutesSpent);
        StepForward();
    }

    public void Rest()
    {
        CurrentFeedback = UserFeedback.Rest();
        StepForward();
    }

    private void ApplyAction()
    {
        if (CurrentFeedback.HasValue)
        {
            var feedback = CurrentFeedback.Value;

            _user.DoTask(feedback);

            if (feedback.Done)
            {
                TaskManager.MarkAsDone(feedback.TaskId, feedback.MinutesSpent);
                var priority = TaskManager[feedback.TaskId].Priority;
                UpdateAdvices(_advisor.SubmitComplition(priority));
            }
            else
            {
                TaskManager.SubmitProgress(feedback.TaskId, feedback.MinutesSpent, feedback.NewEstimate);
                UpdateAdvices(_advisor.GetAdvice());
            }

            _scheduleSource.StepForward(feedback.MinutesSpent);
        }
        else
        {
            _scheduleSource.StepForward(MinQueryMinutes);
            _user.RestFor(MinQueryMinutes);
            UpdateAdvices(_advisor.GetAdvice());
        }
    }

    private void UpdateAdvices(IEnumerable<TTask>? propositions)
    {
        _lastAdvice.Clear();

        if (propositions is not null)
        {
            _lastAdvice.AddRange(propositions);
        }
    }

    public void StartNextDay()
    {
        if (DayGoes)
        {
            throw new InvalidOperationException();
        }

        foreach (var task in TaskGenerator.GenDay())
        {
            TaskManager.Add(task);
        }

        _scheduleSource.StartNextDay();
        _user.StartDay(TaskManager);
        _advisor.StartDay(TaskManager);

        UpdateAdvices(_advisor.GetAdvice());
        CurrentFeedback = _user.GetFeedback(_lastAdvice, TaskManager);

        DayGoes = true;
    }

    public (uint, StatisticsEntry) FinishDay()
    {
        if (!DayGoes)
        {
            throw new InvalidOperationException();
        }

        var entry = _advisor.FinishDay();

        TaskManager.Clear();
        TaskGenerator.ResetCounter();

        DayGoes = false;        

        return entry;
    }
}

public record SimulationConfig<TTask>(
    IUser<TTask> User,
    IAgent Agent,
    ITaskGenerator<TTask> TaskGenerator,
    ITaskManager<TTask> TaskManager,
    SimulatedScheduleSouce ScheduleSouce,
    AdvisorConfig AdvisorConfig
)
    where TTask : ITask;