using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Policies;

public class EpsilonGreedy<TState> : IPolicy<TState>
{
    private readonly double _epsilon;
    private readonly Random _random = new();

    public EpsilonGreedy(double epsilon) => _epsilon = epsilon;

    public int Eval<TQTable>(TState state, TQTable qTable)
        where TQTable : IQFunction<TState>
    {
        var qValues = qTable.GetQValues(state);

        return _random.NextDouble() > _epsilon ? 
            Array.BinarySearch(qValues, qValues.Max()) :
            _random.Next(qValues.Length);
    }
}
