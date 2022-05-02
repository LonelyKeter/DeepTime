namespace DeepTime.Simulation;
using DeepTime.Lib.Data;

public interface ITaskGenerator
{
    Task GenTask();
    Task GenTask(Priority priority, Attractiveness attractiveness);
    Task GenTask(Priority priority, Attractiveness attractiveness, int minutesEstimate);
    IEnumerable<Task> GenTasks(int count)
    {
        for (var i = 0; i < count; i++)
            yield return GenTask();
    }

    IEnumerable<Task> GenDay();

    void ResetCounter();

    bool GeneratesAny();
}
