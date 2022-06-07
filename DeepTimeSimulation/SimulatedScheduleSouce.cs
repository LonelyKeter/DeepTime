namespace DeepTime.Simulation;

using DeepTime.Advisor.Data;

public class SimulatedScheduleSouce : ISimulatedScheduleSource
{
    private TimeBounds _bounds;

    private TimeOnly _lastTime;

    public TimeOnly Start
    {
        get => _bounds.Start;
        set => _bounds.Start = value;
    }
    public TimeOnly End
    {
        get => _bounds.End;
        set => _bounds.End = value;
    }
    public TimeOnly CurrentTime { get; private set; }

    public DayOfWeek DayOfWeek { get; private set; } = DayOfWeek.Monday;
    public HashSet<DayOfWeek> Hollidays { get; init; } = new(7) { DayOfWeek.Saturday, DayOfWeek.Sunday };
    public bool IsHoliday => Hollidays.Contains(DayOfWeek);

    public bool DayHasPassed()
        => CurrentTime > _bounds.End || CurrentTime < _bounds.Start;

    public ScheduleContext GetCurrent()
        => new(DayOfWeek, IsHoliday, CurrentTime, _bounds);

    public void StartNextDay()
    {
        if (CurrentTime >= Start)
        {
            IncrementDay();
        }

        CurrentTime = Start;
        _lastTime = CurrentTime;
    }

    public void StepForward(int minutesCount)
    {
        _lastTime = CurrentTime;
        CurrentTime = CurrentTime.AddMinutes(minutesCount);

        if (CurrentTime < _lastTime)
        {
            IncrementDay();
        }
    }

    private void IncrementDay()
    {
        DayOfWeek = DayOfWeek switch
        {
            DayOfWeek.Monday => DayOfWeek.Tuesday,
            DayOfWeek.Tuesday => DayOfWeek.Wednesday,
            DayOfWeek.Wednesday => DayOfWeek.Thursday,
            DayOfWeek.Thursday => DayOfWeek.Friday,
            DayOfWeek.Friday => DayOfWeek.Saturday,
            DayOfWeek.Saturday => DayOfWeek.Sunday,
            DayOfWeek.Sunday => DayOfWeek.Monday,
            _ => throw new InvalidOperationException("Invalid day of week detected")
        };
    }
}
