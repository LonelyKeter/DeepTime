namespace DeepTime.Simulation;

using DeepTime.Lib;
using DeepTime.Lib.Data;

public class SimulationTool<TAgent, TUser, TTaskGenerator>
    where TAgent: IAgent<State, Advice>
    where TUser: IUser
    where TTaskGenerator: ITaskGenerator
{
    private readonly Advisor<TAgent, SimulatedScheduleSouce> _advisor;

    private readonly TUser _user;    
    private readonly SimulatedScheduleSouce _scheduleSource;

    private readonly List<Task> _lastAdvice = new(); 

    public TaskManager TaskManager { get; } = new();
    public TTaskGenerator TaskGenerator { get; }
    public TAgent Agent 
    { 
        get => _advisor.Agent; 
        set => _advisor.Agent = value; 
    }

    public IReadOnlyList<Task> LastAdvice => _lastAdvice;
    public UserFeedback? CurrentFeedback { get; private set; } = null;
    public ScheduleContext ScheduleContext => _scheduleSource.GetCurrent();

    public bool DayGoes { get; private set; } = false;
    public int MinQueryMinutes { get; set; } = 10;

    public SimulationTool(SimulationConfig<TAgent, TUser, TTaskGenerator> config)
    {
        _user = config.User;
        _scheduleSource = config.ScheduleSouce;
        _advisor = new(config.Agent, config.ScheduleSouce, config.AdvisorConfig);

        TaskGenerator = config.TaskGenerator;
    }

    public bool StepForward()
    {
        ApplyAction();
        UpdateAdvices();

        CurrentFeedback = _user.GetFeedback(_lastAdvice, TaskManager.GetUndone());

        return !TaskManager.GetUndone().Any();
    }    

    public void SimulateDay()
    {
        StartNextDay();
        while(!_scheduleSource.DayHasPassed())
        {
            if (StepForward()) break;
        }
        FinishDay();
    }

    public void SimulateDays(int dayCount) 
    {
        for (var i = 0; i < dayCount; i++)
        {
            SimulateDay();
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

            if (feedback.Done)
                TaskManager.MarkAsDone(feedback.TaskId, feedback.MinutesSpent);
            else 
                TaskManager.SubmitProgress(feedback.TaskId, feedback.MinutesSpent, feedback.NewEstimate);

            _scheduleSource.StepForward(feedback.MinutesSpent);
            _user.DoTask(feedback);
        }
        else
        {
            _scheduleSource.StepForward(MinQueryMinutes);
            _user.RestFor(MinQueryMinutes);
        }
    }

    private void UpdateAdvices()
    {
        _lastAdvice.Clear();
        var advice = _advisor.GetAdvice(TaskManager);

        if (advice is not null)
        {
            _lastAdvice.AddRange(advice);
        }
    }

    public void StartNextDay()
    {
        if (DayGoes)
        {
            FinishDay();
            return;
        }

        foreach (var task in TaskGenerator.GenDay())
        {
            TaskManager.Add(task);
        }

        _scheduleSource.StartNextDay();
        _user.StartDay(TaskManager);
        _advisor.StartDay(TaskManager);

        DayGoes = true;
    }
    
    public void FinishDay()
    {
        if (!DayGoes) 
        {
            return;
        }

        _advisor.FinishDay(TaskManager); 

        TaskManager.Clear();
        TaskGenerator.ResetCounter();

        DayGoes = false;
    }
}

public record SimulationConfig<TAgent, TUser, TTaskGenerator>(
    TUser User, 
    TAgent Agent, 
    TTaskGenerator TaskGenerator,
    SimulatedScheduleSouce ScheduleSouce, 
    AdvisorConfig AdvisorConfig
)
    where TAgent : IAgent<State, Advice>
    where TUser : IUser
    where TTaskGenerator : ITaskGenerator;