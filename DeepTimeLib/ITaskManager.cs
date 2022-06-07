namespace DeepTime.Advisor;

using Data;

using System.Collections.Generic;

public interface ITaskManager<TTask> : IEnumerable<TTask> where TTask : ITask
{
    int Count { get; }

    TTask this[int id] { get; }
    IReadOnlyList<TTask> this[Priority pr, Attractiveness attr] { get; }

    IEnumerable<TTask> GetUndone();
    IEnumerable<TTask> GetUndone(Priority pr, Attractiveness attr);

    IEnumerable<TTask> GetDone();
    IEnumerable<TTask> GetDone(Priority pr, Attractiveness attr);

    void Add(TTask task);
    TTask Remove(int id);

    void SubmitProgress(int id, int minutesSpent, int? minutesLeft);
    void MarkAsDone(int id, int minutesSpent);

    bool Contains(int id);
    IEnumerable<TTask> Clear();
}
