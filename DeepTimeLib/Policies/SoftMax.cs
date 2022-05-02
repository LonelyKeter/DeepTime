using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Policies;

public class SoftMax<TState> : IPolicy<TState>
{
    private readonly Random _random = new();

    public double Temperature { get; set; }

    public SoftMax(double temperature) => Temperature = temperature;

    public int Eval<TQTable>(TState state, TQTable qTable)
        where TQTable : IQFunction<TState>
    {
        var sum = 0.0;

        var qValues = qTable
            .GetQValues(state)
            .Select(q =>
            {
                sum += Math.Exp(q) / Temperature;
                return sum;
            })
            .ToArray();

        var rand = _random.NextDouble() * sum;
        return qValues.TakeWhile(q => q < rand).Count();
    }
}
