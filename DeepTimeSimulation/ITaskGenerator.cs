namespace DeepTime.Simulation;
using DeepTime.Lib.Data;

public interface ITaskGenerator
{
    Task GenTask();
    List<Task> GenTasks();
}
