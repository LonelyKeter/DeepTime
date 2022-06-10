using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulation.Users;

using DeepTime.Advisor.Data;

internal abstract class UserBase<TTask> : IUser<TTask>
    where TTask : ITask
{
    public abstract void DoTask(UserFeedback feedback);
    public abstract UserFeedback? GetFeedback<P, T>(P? propositions, T tasks)
        where P : IReadOnlyList<TTask>
        where T : IEnumerable<TTask>;
    public abstract void RestFor(int minutes);
    public abstract void StartDay(IEnumerable<TTask> tasks);
}
