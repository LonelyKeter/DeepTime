namespace DeepTime.Advisor.Data;

public static class Types
{
    public static readonly int PriorityCount = Enum.GetValues<Priority>().Length;
    public static readonly IReadOnlyCollection<Priority> PriorityValues = Array.AsReadOnly(Enum.GetValues<Priority>());

    public static int AsIndex(this Priority priority) => (int)priority;
    public static Priority PriorityFromIndex(int index) 
        => index is >= (int)Priority.VeryLow and <= (int)Priority.VeryHigh ?
            (Priority)index :
            throw new ArgumentOutOfRangeException(nameof(index));

    public static readonly int AttractivenessCount = Enum.GetValues<Attractiveness>().Length;
    public static readonly IReadOnlyCollection<Attractiveness> AttractivenessesValues = Array.AsReadOnly(Enum.GetValues<Attractiveness>());

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


public class Task: ITask
{
    public int Id { get; set; }
    public Attractiveness Attractiveness { get; set; }
    public Priority Priority { get; set; }
    public int MinutesEstimate { get; set; }
    public int MinutesSpent { get; set; }
    public bool Done { get; set; }

    public Task(int id, Attractiveness attractiveness, Priority priority, int minutesEstimate, int minutesSpent, bool done)
    {
        Id = id;
        Attractiveness = attractiveness;
        Priority = priority;
        MinutesEstimate = minutesEstimate;
        MinutesSpent = minutesSpent;
        Done = done;
    }
}

public interface ITask
{
    public int Id { get; set; }
    public Attractiveness Attractiveness { get; set; }
    public Priority Priority { get; set; }
    public int MinutesEstimate { get; set; }
    public int MinutesSpent { get; set; }
    public bool Done { get; set; }

    public int LeftEstimate() => LeftEstimate(0);
    public int LeftEstimate(int minutesSpent) => MinutesEstimate - (MinutesSpent + minutesSpent);

    public bool SupposedlyDone() => SupposedlyDone(0);
    public bool SupposedlyDone(int minutesSpent) => MinutesEstimate <= MinutesSpent + minutesSpent;

    public double DoneWorkRatio() => DoneWorkRatio(0);
    public double DoneWorkRatio(int minutesSpent) => (double)(MinutesSpent + minutesSpent) / MinutesEstimate;
}

public record ScheduleContext(DayOfWeek DayOfWeek, bool IsHolliday, TimeOnly Time, TimeBounds Bounds);
public record struct WorkloadContextEntry(int MinutesEstimate, int Count);
public record struct State(WorkloadContext TODO, WorkloadContext Done, ScheduleContext ScheduleContext);
public record struct Advice(Priority Priority, Attractiveness Attractiveness, bool Rest);

public record struct TimeBounds(TimeOnly Start, TimeOnly End);