namespace DeepTime.Simulation;

using DeepTime.Lib;
public interface ISimulatedScheduleSource: IScheduleSource
{
    TimeOnly CurrentTime { get; }

    void StepForward(int minutesCount);
    void StartNextDay();
}
