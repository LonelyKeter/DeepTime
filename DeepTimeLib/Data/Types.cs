namespace DeepTime.Lib.Data;

public enum Priority
{
    VeryLow = 1,
    Low,
    Medium,
    High,
    VeryHigh,
}

public enum Attractiveness
{
    VeryLow = 1,
    Low,
    Medium,
    High,
    VeryHigh,
}

public record struct Task(int Id, Attractiveness Attractiveness, Priority Priority, int MinutesEstimate, int MinutesSpent, bool Done);
public record struct ScheduleContext(DayOfWeek DayOfWeek, bool IsHolliday, TimeOnly Time, TimeBounds Bounds);
public record struct WorkloadContextEntry(int MinutesEstimate, int Count);
public record struct WorkloadContext(WorkloadContextEntry[,] MinutesEstimate);
public record struct State(WorkloadContext TODO, WorkloadContext Done, ScheduleContext ScheduleContext);
public record struct Action(Priority Priority, Attractiveness Attractiveness, bool Rest);

public record struct TimeBounds(TimeOnly Start, TimeOnly End);