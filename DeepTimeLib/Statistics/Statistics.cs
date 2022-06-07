namespace DeepTime.Advisor.Statistics;

using DeepTime.Advisor.Data;

public class Statistics : IStatistics
{
    private List<StatisticsEntry> _stats = new();
    private MedianHeap<double> _medianHeap = new();

    public int Count => _stats.Count;
    
    public double? Median => _medianHeap.PeekMedian();
    public double? Average { get; private set; } = default;
    public double? Min { get; private set; } = default;
    public double? Max { get; private set; } = default;
    public IReadOnlyList<StatisticsEntry> Stats => _stats;

    public void Submit(StatisticsEntry entry)
    {
        _stats.Add(entry);
        _medianHeap.Push(entry.EpisodeReward);

        if (!(Min <= entry.EpisodeReward))
            Min = entry.EpisodeReward;

        if (!(Max >= entry.EpisodeReward))
            Max = entry.EpisodeReward;

        if (Average.HasValue)
        {
            Average = Average * ((double)(Count - 1) / Count) + entry.EpisodeReward / Count;
        }
        else
        {
            Average = entry.EpisodeReward;
        }
    }

    public void Clear()
    {
        _stats.Clear();
        _medianHeap.Clear();

        Min = Max = default;
    }
}

public record struct StatisticsEntry(TaskEntry[] TasksDone, double EpisodeReward)
{
    public int AllDone => TasksDone.Sum(a => a.Done);
    public int AllTodo => TasksDone.Sum(a => a.Todo);

    public TaskEntry Total => new(AllDone, AllTodo);
}

public record struct TaskEntry(int Done, int Todo);