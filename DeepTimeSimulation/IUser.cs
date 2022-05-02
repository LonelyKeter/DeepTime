namespace DeepTime.Simulation;
using DeepTime.Lib.Data;

public interface IUser
{
    UserFeedback? GetFeedback<P, T>(P? propositions, T tasks)
        where P : IEnumerable<Task>
        where T : IEnumerable<Task>;

    void DoTask(UserFeedback feedback);
    void RestFor(int minutes);
    void StartDay(IEnumerable<Task> tasks);
}

public record struct UserFeedback(int TaskId, int MinutesSpent, bool Done, int? NewEstimate)
{
    public static UserFeedback DoTask(int id, int minutesSpent, int? newEstimate = null) 
        => new(id, minutesSpent, false, newEstimate);

    public static UserFeedback FinishTask(int id, int minutesSpent)
        => new(id, minutesSpent, true, null);

    public static UserFeedback? Rest() => null;
}