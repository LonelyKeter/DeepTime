
namespace DeepTime.Lib;

using System.Collections.Generic;
using Data;

using Task = Data.Task;

public interface ITaskManager : IEnumerable<Task>
{
    Task this[int id] { get; }
    IReadOnlyList<Task> this[Priority pr, Attractiveness attr] { get; }

    IEnumerable<Task> GetUndone();
    IEnumerable<Task> GetUndone(Priority pr, Attractiveness attr);

    IEnumerable<Task> GetDone();
    IEnumerable<Task> GetDone(Priority pr, Attractiveness attr);

    void Add(Task task);
    void Remove(int id);

    void SubmitProgress(int id, int minutesSpent, int? minutesLeft);
    void MarkAsDone(int id, int minutesSpent);

    bool Contains(int id);
    void Clear();
}
