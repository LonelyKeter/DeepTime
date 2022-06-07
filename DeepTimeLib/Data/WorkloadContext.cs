using System.Collections;

using static DeepTime.Advisor.Data.Types;

namespace DeepTime.Advisor.Data;

public struct WorkloadContext : IEnumerable<WorkloadContextEntry>
{
    public static readonly int ArrayPresentationLength = PriorityCount * AttractivenessCount;

    private readonly WorkloadContextEntry[,] _inner =
        new WorkloadContextEntry[PriorityCount, AttractivenessCount];

    public WorkloadContext() { }

    public static WorkloadContext GetDone<TManager, TTask>(TManager tasks)
        where TManager : ITaskManager<TTask>
        where TTask : ITask

    {
        var context = new WorkloadContext();

        foreach (var pr in Enum.GetValues<Priority>())
            foreach (var attr in Enum.GetValues<Attractiveness>())
            {
                var (minutesSpent, count) = (0, 0);
                var values = tasks[pr, attr];

                foreach (var task in values)
                {
                    minutesSpent += task.MinutesSpent;
                    count += task.Done ? 1 : 0;
                }

                context[pr, attr] = new(minutesSpent, count);
            }

        return context;
    }

    public static WorkloadContext GetTodo<TManager, TTask>(TManager tasks) 
        where TManager : ITaskManager<TTask>
        where TTask : ITask
    {
        var context = new WorkloadContext();

        foreach (var pr in Enum.GetValues<Priority>())
            foreach (var attr in Enum.GetValues<Attractiveness>())
            {
                var (minutesSpent, count) = (0, 0);
                var values = tasks[pr, attr];

                foreach (var task in values)
                {
                    minutesSpent += task.MinutesEstimate;
                    count += 1;
                }

                context[pr, attr] = new(minutesSpent, count);
            }

        return context;
    }

    public static (WorkloadContext, WorkloadContext) GetTodoAndDone<TManager, TTask>(TManager tasks)
        where TManager : ITaskManager<TTask>
        where TTask : ITask
    {
        var (todo, done) = (new WorkloadContext(), new WorkloadContext());

        foreach (var pr in Enum.GetValues<Priority>())
            foreach (var attr in Enum.GetValues<Attractiveness>())
            {
                var minutesSpent = 0;
                var minutesEstimate = 0;
                var count = 0;
                var doneCount = 0;

                var values = tasks[pr, attr];

                foreach (var task in values)
                {
                    minutesSpent += task.MinutesSpent;
                    minutesEstimate += task.MinutesEstimate;
                    count += 1;
                    doneCount += task.Done ? 1 : 0;
                }

                todo[pr, attr] = new(minutesEstimate, count);
                done[pr, attr] = new(minutesSpent, doneCount);
            }

        return (todo, done);
    }


    public IEnumerator<WorkloadContextEntry> GetEnumerator()
        => _inner.Cast<WorkloadContextEntry>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _inner.GetEnumerator();

    public WorkloadContextEntry this[Priority pr, Attractiveness attr]
    {
        get => _inner[pr.AsIndex(), attr.AsIndex()];
        set => _inner[pr.AsIndex(), attr.AsIndex()] = value;
    }    
}
