namespace DeepTime.Simulation;
using DeepTime.Lib.Data;

public interface IUser
{   
    UserFeedback? GetFeedback(IEnumerable<Task>? propositions, TimeOnly time);
    void StartDay(TimeOnly time, IEnumerable<Task> tasks);
}

public record struct UserFeedback(int TaskId, int MinutesSpent, bool Done, int NewEstimate);
