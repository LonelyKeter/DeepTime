namespace DeepTime.Simulation;

using DeepTime.Advisor;

public interface ISimulatedScheduleSource: IScheduleSource
{
    TimeOnly CurrentTime { get; }

    void StepForward(int minutesCount);
    void StartNextDay();
}
