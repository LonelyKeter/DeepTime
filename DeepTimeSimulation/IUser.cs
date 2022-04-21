namespace DeepTime.Simulation;
using DeepTime.Lib.Data;

public interface IUser
{
    List<Task> InitDay();
    UserFeedback? GetFeedback(IEnumerable<Task>? propositions, TimeOnly time);
}

public record struct UserFeedback(int TaskId, int MinutesSpent, bool Done, int NewEstimate);
