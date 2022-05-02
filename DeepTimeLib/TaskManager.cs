using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepTime.Lib.Data;
using static DeepTime.Lib.Data.Types;
using Task = DeepTime.Lib.Data.Task;

namespace DeepTime.Lib;

public class TaskManager : ITaskManager
{
    private readonly Dictionary<int, Task> _taskDictionary = new();
    private readonly List<Task>[,] _taskTable;

    public TaskManager()
    {
        _taskTable = new List<Task>[PriorityCount, AttractivenessCount];

        for (var p = 0; p < PriorityCount; p++)
            for (var a = 0; a < AttractivenessCount; a++)
            {
                _taskTable[p, a] = new();
            }
    }

    public Task this[int id] => _taskDictionary[id];
    public IReadOnlyList<Task> this[Priority pr, Attractiveness attr] => Get(pr, attr);

    public IEnumerable<Task> GetUndone()
        => _taskDictionary.Values.Where(task => !task.Done);

    public IEnumerable<Task> GetUndone(Priority pr, Attractiveness attr)
        => Get(pr, attr).Where(task => !task.Done);

    public IEnumerable<Task> GetDone()
        => _taskDictionary.Values.Where(task => task.Done);

    public IEnumerable<Task> GetDone(Priority pr, Attractiveness attr)
        => Get(pr, attr).Where(task => task.Done);

    public void Add(Task task)
    {
        _taskDictionary.Add(task.Id, task);
        Get(task.Priority, task.Attractiveness).Add(task); 
    }

    public void Remove(int id)
    {
        _taskDictionary.Remove(id, out var task);
        Get(task.Priority, task.Attractiveness).Remove(task);
    }

    public void Clear() => _taskDictionary.Clear();
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

    public IEnumerator<Task> GetEnumerator()
        => _taskDictionary.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private List<Task> Get(Priority pr, Attractiveness attr) 
        => _taskTable[pr.AsIndex(), attr.AsIndex()];
}
