namespace DeepTime.Simulation; 

using DeepTime.Advisor.Data;

using Formatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;



[Serializable()]
public class User<TTask> : IUser<TTask> where TTask: ITask
{
    private static readonly Random _random = new();

    private UserState _state = InitialUserState;
    public UserConfig Config { get; private init; }

    public UserState State => _state;

    public bool CanWork => CanWorkInState(_state);
    public bool WantsToWork => WantsToWorkInState(_state);

    public User(UserConfig config)
    {
        Config = config;
    }

    public UserFeedback? GetFeedback<P, T>(P? propositions, T tasks)
        where P : IReadOnlyList<TTask>
        where T : IEnumerable<TTask>
    {
        if (!CanWork) return null;

        if (Config.Strategy == UserStrategy.AcceptAll)
        {
            return ChooseTaskRandomly(propositions);
        }

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
            _state.MinutesWorkedContiniously = 0;
        }
        else
        {
            if (taskId == _state.LastTaskId)
                _state.MinutesWorkedOnLastTask += minutesSpent;
            else
            {
                _state.LastTaskId = taskId;
                _state.MinutesWorkedOnLastTask = minutesSpent;
            }

            _state.MinutesWorkedContiniously += minutesSpent;
        }
    }

    public void StartDay(IEnumerable<TTask> _)
    {
        _state = InitialUserState;
    }

    private UserFeedback? ChooseTaskIndependently<T>(T tasks)
        where T : IEnumerable<TTask>
    {
        return Config.Strategy switch
        {
            UserStrategy.AttractiveFirst => ChooseTaskAttractiveFirst(tasks),
            UserStrategy.PriorFirst => ChooseTaskPriorFirst(tasks),
            UserStrategy.AcceptAll => null,
            _ => throw new InvalidOperationException("Invalid user strategy.")
        };
    }


    private UserFeedback? ChooseTaskAttractiveFirst<T>(T tasks)
        where T : IEnumerable<TTask>
    {
        TTask? max = default;
        var maxAttractiveness = Attractiveness.VeryLow;

        foreach (var task in tasks.Where(task => !task.Done))
        {
            if (task.Attractiveness >= maxAttractiveness)
            {
                maxAttractiveness = task.Attractiveness;
                max = task;
            }
        }

        return max is not null ? PrepareFeedback(max) : null;
    }

    private UserFeedback? ChooseTaskPriorFirst<T>(T tasks)
        where T : IEnumerable<TTask>
    {
        TTask? max = default;
        var maxPriority = Priority.VeryLow;

        foreach (var task in tasks.Where(task => !task.Done))
        {
            if (task.Priority >= maxPriority)
            {
                maxPriority = task.Priority;
                max = task;
            }
        }

        return max is not null ? PrepareFeedback(max) : null;
    }

    private UserFeedback? ChooseTaskRandomly<T>(T? tasks)
        where T: IReadOnlyList<TTask>
    {
        if (tasks is T collection)
        {
            return PrepareFeedback(collection[new Random().Next(0, collection.Count)]);
        }
        else
        {
            return null;
        }
    }

    private bool ChooseTaskFromPropositions<P>(P propositions, out UserFeedback? feedback)
        where P : IReadOnlyCollection<TTask>
    {
        var choice = propositions
            .Cast<TTask?>()
            .FirstOrDefault(IsPropositonAcceptable);

        if (choice is not null)
        {
            feedback = PrepareFeedback(choice);
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

    private UserFeedback PrepareFeedback(TTask task)
    {
        var minutesSpent = Math.Min(
            task.Attractiveness >= Attractiveness.High ? 30 : 15,
            task.LeftEstimate()
        );

        var estimateIsCorrect = EstimateIsCorrect();

        if (estimateIsCorrect && task.SupposedlyDone(minutesSpent) || !estimateIsCorrect && task.DoneWorkRatio(minutesSpent) >= 0.85)
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
        && state.MinutesRested >= Math.Max(Config.MinRest, state.MinutesWorkedContiniously / 2);

    public bool WantsToWorkInState(UserState state)
        => (double)state.MinutesWorkedContiniously / Config.MaxContiniousWorkMinutes <= Config.Initiativeness;

    private bool IsPropositonAcceptable(TTask prop)
    {
        if (Config.Strategy == UserStrategy.AcceptAll)
        {
            return true;
        }

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

[Serializable()]
public enum UserStrategy
{
    AttractiveFirst,
    PriorFirst,
    AcceptAll
}

public interface IUserConfig
{
    double EstimateAccuracy { get; set; }
    double Initiativeness { get; set; }
    int MaxContiniousWorkMinutes { get; set; }
    int MaxMinutesOnOneTask { get; set; }
    int MaxWorkMinutes { get; set; }
    int MinRest { get; set; }
    UserStrategy Strategy { get; set; }

    void LoadFrom(Stream stream);
}

[Serializable()]
public class UserConfig : ICloneable, RL.ISerializable<UserConfig>, IUserConfig
{
    private static readonly Formatter Formatter = new();

    public int MaxWorkMinutes { get; set; }
    public double Initiativeness { get; set; }
    public int MaxContiniousWorkMinutes { get; set; }
    public int MaxMinutesOnOneTask { get; set; }
    public int MinRest { get; set; }
    public double EstimateAccuracy { get; set; }
    public UserStrategy Strategy { get; set; }

    public UserConfig(
        int maxWorkMinutes,
        double initiativeness,
        int maxContiniousWorkMinutes,
        int maxMinutesOnOneTask,
        int minRest,
        double estimateAccuracy,
        UserStrategy strategy
    )
    {
        MaxWorkMinutes = maxWorkMinutes;
        Initiativeness = initiativeness;
        MaxContiniousWorkMinutes = maxContiniousWorkMinutes;
        MaxMinutesOnOneTask = maxMinutesOnOneTask;
        MinRest = minRest;
        EstimateAccuracy = estimateAccuracy;
        Strategy = strategy;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public static UserConfig Deserialize(Stream stream)
    {
        return (UserConfig)Formatter.Deserialize(stream);
    }

    public void LoadFrom(Stream stream)
    {
        var instance = Deserialize(stream);

        MaxWorkMinutes = instance.MaxWorkMinutes;
        Initiativeness = instance.Initiativeness;
        MaxContiniousWorkMinutes = instance.MaxContiniousWorkMinutes;
        MaxMinutesOnOneTask = instance.MaxMinutesOnOneTask;
        MinRest = instance.MinRest;
        EstimateAccuracy = instance.EstimateAccuracy;
        Strategy = instance.Strategy;
    }

    public void Serialize(Stream stream)
    {
        Formatter.Serialize(stream, this);
    }

    public static UserConfig Default => new(
            maxWorkMinutes: 60 * 8,
            initiativeness: 0.3,
            maxContiniousWorkMinutes: 150,
            maxMinutesOnOneTask: 90,
            minRest: 20,
            estimateAccuracy: 0.8,
            strategy: UserStrategy.PriorFirst
    );
}

[Serializable()]
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