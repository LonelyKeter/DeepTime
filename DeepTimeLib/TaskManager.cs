using DeepTime.Advisor.Data;

using System.Collections;

using static DeepTime.Advisor.Data.Types;

namespace DeepTime.Advisor;

public class TaskManager<TTask> : ITaskManager<TTask> where TTask : ITask
{
    private Dictionary<int, TTask> _taskDictionary = new();
    private readonly List<TTask>[,] _taskTable;

    public int Count => _taskDictionary.Count;

    public TaskManager()
    {
        _taskTable = new List<TTask>[PriorityCount, AttractivenessCount];

        for (var p = 0; p < PriorityCount; p++)
            for (var a = 0; a < AttractivenessCount; a++)
            {
                _taskTable[p, a] = new();
            }
    }

    public TTask this[int id] => _taskDictionary[id];
    public IReadOnlyList<TTask> this[Priority pr, Attractiveness attr] => Get(pr, attr);

    public IEnumerable<TTask> GetUndone()
        => _taskDictionary.Values.Where(task => !task.Done);

    public IEnumerable<TTask> GetUndone(Priority pr, Attractiveness attr)
        => Get(pr, attr).Where(task => !task.Done);

    public IEnumerable<TTask> GetDone()
        => _taskDictionary.Values.Where(task => task.Done);

    public IEnumerable<TTask> GetDone(Priority pr, Attractiveness attr)
        => Get(pr, attr).Where(task => task.Done);

    public void Add(TTask task)
    {
        _taskDictionary.Add(task.Id, task);
        Get(task.Priority, task.Attractiveness).Add(task);
    }

    public TTask Remove(int id)
    {
        _taskDictionary.Remove(id, out var task);
        Get(task.Priority, task.Attractiveness).Remove(task);

        return task;
    }

    public IEnumerable<TTask> Clear()
    {
        var cleared = _taskDictionary.Values;
        _taskDictionary = new();

        foreach (var list in _taskTable)
        {
            list.Clear();
        }

        return cleared;
    }
    public bool Contains(int id) => _taskDictionary.ContainsKey(id);

    public void SubmitProgress(int id, int minutesSpent, int? minutesLeft)
    {
        var task = _taskDictionary[id];

        task.MinutesSpent += minutesSpent;
        if (minutesLeft is not null)
            task.MinutesEstimate = task.MinutesSpent + minutesLeft.Value;

        _taskDictionary[id] = task;
    }

    public void MarkAsDone(int id, int minutesSpent)
    {
        var task = _taskDictionary[id];

        task.MinutesSpent += minutesSpent;
        task.MinutesEstimate = task.MinutesSpent;
        task.Done = true;

        _taskDictionary[id] = task;
    }

    public IEnumerator<TTask> GetEnumerator()
        => _taskDictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private List<TTask> Get(Priority pr, Attractiveness attr)
        => _taskTable[pr.AsIndex(), attr.AsIndex()];
}
