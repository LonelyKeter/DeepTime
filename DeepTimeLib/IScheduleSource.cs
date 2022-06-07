namespace DeepTime.Advisor;

public interface IScheduleSource
{
    Data.ScheduleContext GetCurrent();
    bool DayHasPassed();
}
