namespace DeepTime.Simulation;

using DeepTime.Advisor.Data;

public interface ITaskGenerator<TTask> where TTask: ITask
{
    TTask GenTask();
    TTask GenTask(Priority priority, Attractiveness attractiveness);
    TTask GenTask(Priority priority, Attractiveness attractiveness, int minutesEstimate);
    IEnumerable<TTask> GenTasks(int count)
    {
        for (var i = 0; i < count; i++)
            yield return GenTask();
    }

    IEnumerable<TTask> GenDay();

    void ResetCounter();

    bool GeneratesAny();
}
