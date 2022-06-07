namespace DeepTime.RL.Policies;

[Serializable()]
public class EpsilonGreedy : IPolicy
{
    private static readonly Random _random = new();

    private readonly double _epsilon;

    public EpsilonGreedy(double epsilon) => _epsilon = epsilon;

    public int Eval(double[] actionValues)
    {
        return _random.NextDouble() > _epsilon ? 
            Array.BinarySearch(actionValues, actionValues.Max()) :
            _random.Next(actionValues.Length);
    }
}
