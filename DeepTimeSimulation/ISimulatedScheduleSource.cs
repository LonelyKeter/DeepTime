namespace DeepTime.Simulation;

using DeepTime.Lib;
public interface ISimulatedScheduleSource: IScheduleSource
{
    bool StepForward(int minutesCount);
    TimeOnly GetCurrentTime();
    void StartNextDay();
}
