using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DeepTime.Lib.Data.Types;

namespace DeepTime.Lib.Data;

public struct WorkloadContext : IEnumerable<WorkloadContextEntry>
{
    public static readonly int ArrayPresentationLength = PriorityCount * AttractivenessCount;

    private readonly WorkloadContextEntry[,] _inner =
        new WorkloadContextEntry[PriorityCount, AttractivenessCount];

    public WorkloadContext() { }

    public static WorkloadContext GetDone<T>(T tasks) where T : ITaskManager
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

    public static WorkloadContext GetTodo<T>(T tasks) where T : ITaskManager
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
