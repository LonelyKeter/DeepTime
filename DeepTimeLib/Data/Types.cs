namespace DeepTime.Lib.Data;

public static class Types
{
    public static readonly int PriorityCount = Enum.GetValues<Priority>().Length;
    public static int AsIndex(this Priority priority) => (int)priority;
    public static Priority PriorityFromIndex(int index) 
        => index is >= (int)Priority.VeryLow and <= (int)Priority.VeryHigh ?
        (Priority)index :
        throw new ArgumentOutOfRangeException(nameof(index));

    public static readonly int AttractivenessCount = Enum.GetValues<Attractiveness>().Length;
    public static int AsIndex(this Attractiveness attractiveness) => (int)attractiveness;
    public static Attractiveness AttractivenessFromIndex(int index)
        => index is >= (int)Attractiveness.VeryLow and <= (int)Attractiveness.VeryHigh ?
        (Attractiveness)index :
        throw new ArgumentOutOfRangeException(nameof(index));
}

public enum Priority
{
    VeryLow = 0,
    Low,
    Medium,
    High,
    VeryHigh,
}

public enum Attractiveness
{
    VeryLow = 0,
    Low,
    Medium,
    High,
    VeryHigh,
}

public record struct Task(int Id, Attractiveness Attractiveness, Priority Priority, int MinutesEstimate, int MinutesSpent, bool Done)
{
    public int LeftEstimate => MinutesEstimate - MinutesSpent;
    public bool SupposedlyDone => MinutesEstimate <= MinutesSpent;
    public double DoneWorkPercentage => (double)MinutesSpent / MinutesEstimate * 100;
}
public record struct ScheduleContext(DayOfWeek DayOfWeek, bool IsHolliday, TimeOnly Time, TimeBounds Bounds);
public record struct WorkloadContextEntry(int MinutesEstimate, int Count);
public record struct State(WorkloadContext TODO, WorkloadContext Done, ScheduleContext ScheduleContext);
public record struct Advice(Priority Priority, Attractiveness Attractiveness, bool Rest);

public record struct TimeBounds(TimeOnly Start, TimeOnly End);