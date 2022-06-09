namespace DeepTime.Advisor.Statistics;

using DeepTime.Advisor.Data;

using System.Collections.Generic;
using System.Linq;

public class Statistics : IStatistics
{
    private readonly Queue<StatisticsEntry> _history = new();
    private readonly MedianList<double> _medianList = new();

    public double AverageTaskComplition(Priority pr) => _history.Select(entry => entry.TasksDone[pr.AsIndex()].Percentage).Average();
    public double AverageTaskComplition() => _history.Select(entry => entry.Total.Percentage).Average();

    public int Count => _history.Count;

    private int _accountFor = 100;
    public int AccountFor
    {
        get => _accountFor;
        set
        {
            if (_accountFor != value)
            {
                _accountFor = value;
                TrimExcess();
            }
        }
    }

    public double? MedianReward => _medianList.Median;
    public double? AverageReward => _history.Count > 0 ? _history.Select(entry => entry.EpisodeReward).Average() : null;

    public StatisticValues? StatisticValues => Count == 0 ? null : new(
        MedianReward.Value,
        AverageReward.Value,
        Types.PriorityValues.Select(AverageTaskComplition).ToArray(),
        AverageTaskComplition());

    public void Submit(StatisticsEntry entry)
    {
        AddEntryData(entry);
    }

    public void Clear()
    {
        _history.Clear();
        _medianList.Clear();
    }

    private void AddEntryData(StatisticsEntry entry)
    {
        TrimExcess();

        _history.Enqueue(entry);
        _medianList.Add(entry.EpisodeReward);
    }

    private void TrimExcess()
    {
        if (Count < AccountFor || AccountFor == 0) return;

        var excess = Math.Max(Count - AccountFor, 1);

        foreach (var _ in Enumerable.Range(0, excess))
        {
            var removed = _history.Dequeue().EpisodeReward;
            _medianList.Remove(removed);
        }
    }
}

public record struct StatisticValues(
    double MedianReward,
    double AverageReward,
    double[] AverageTaskComplition,
    double TotalTaskComplition);

public record struct StatisticsEntry(TaskEntry[] TasksDone, double EpisodeReward)
{
    public int AllDone => TasksDone.Sum(a => a.Done);
    public int AllTodo => TasksDone.Sum(a => a.Todo);

    public TaskEntry Total => new(AllDone, AllTodo);
}

public record struct TaskEntry(int Done, int Todo)
{
    public double Percentage => Done * 100.0 / Todo;
}