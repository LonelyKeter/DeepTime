using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Policies;

public class SoftMax<TState> : IPolicy<TState>
{
    private readonly double _temperature;
    private readonly Random _random = new();

    public SoftMax(double temperature) => _temperature = temperature;

    public int Eval<TQTable>(TState state, TQTable qTable)
        where TQTable : IQFunction<TState>
    {
        var qValues = qTable
            .GetQValues(state)
            .Select(q => (float)(Math.Exp(q) / _temperature))
            .ToArray();

        var sum = qValues.Sum();
        var rand = _random.NextDouble() * sum;

        return qValues.TakeWhile(q => q < rand).Count();
    }
}
