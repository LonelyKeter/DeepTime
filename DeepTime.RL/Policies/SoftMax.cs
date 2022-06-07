namespace DeepTime.RL.Policies;

[Serializable()]
public class SoftMax : IPolicy
{
    private static readonly Random _random = new();
    public double Temperature { get; set; }

    public SoftMax(double temperature) => Temperature = temperature;

    public int Eval(double[] actionValues)
    {
        var sum = 0.0;

        var qValues = actionValues
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
