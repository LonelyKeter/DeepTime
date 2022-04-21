namespace DeepTime.Lib;

public interface IScheduleSource
{
    Data.ScheduleContext GetCurrent();
    bool DayHasPassed();
}
