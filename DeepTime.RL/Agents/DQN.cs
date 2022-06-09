namespace DeepTime.RL.Agents;

using DeepTime.RL.Aproximators;

using Formatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

[Serializable()]
public class DQN : IAgent, ISerializable<DQN>
{
    private static readonly Formatter Formatter = new();

    private HyperParameters _hyperParameters;

    private IPolicy _policy;
    private QFunctionApproximator _qApproximator;

    public HyperParameters HyperParameters => _hyperParameters;

    public int StateVecLength => _qApproximator.StateVecLength;
    public int ActionCount => _qApproximator.ActionCount;

    public void SetLearningRate(double rate) => _hyperParameters.LearningRate = rate;
    public void SetDiscountFactor(double factor) => _hyperParameters.DiscountFactor = factor;

    private bool InEpisode => _lastState is not null;

    public uint EpisodeNumber { get; private set; } = 0;

    private int? _lastAction;
    private double[]? _lastState;

    public DQN(IPolicy policy, QFunctionApproximator qApproximator, HyperParameters hyperParameters)
    {
        _policy = policy;
        _qApproximator = qApproximator;
        _hyperParameters = hyperParameters;
    }

    public void EndEpisode(double[] state, double reward)
    {
        if (!InEpisode)
            throw new InvalidOperationException("Episode wasn't started.");

        CorrectQValue(state, reward, true);

        _lastAction = null;
        _lastState = null;
        EpisodeNumber++;
    }

    public void StartEpisode(double[] state)
    {
        _lastState = state;
    }

    public int Eval()
    {
        AssertEpisodeActive();

        _lastAction = _policy.Eval(_qApproximator.GetQValues(_lastState));
        return _lastAction.Value;
    }

    public void SetNext(double[] state, double reward)
    {
        AssertEpisodeActive();

        CorrectQValue(state, reward);
        _lastState = state;
    }

    private void CorrectQValue(double[] newState, double reward, bool terminal = false)
    {
        if (_lastAction.HasValue)
        {
            var advice = _lastAction.Value;

            var correction = terminal ? reward : reward + _hyperParameters.DiscountFactor * _qApproximator.GetQValues(newState).Max();
            _qApproximator.CorrectQValue(_lastState, advice, correction);
        }
    }

    public static DQN Deserialize(Stream stream)
    {
        return (DQN)Formatter.Deserialize(stream);
    }

    public void LoadFrom(Stream stream)
    {
        var instance = Deserialize(stream);
        
        _hyperParameters = instance._hyperParameters;
        _qApproximator = instance._qApproximator;
        _policy = instance._policy;
    }

    public void Serialize(Stream stream)
    {
        Formatter.Serialize(stream, this);
    }

    #region Assertions
    private void AssertEpisodeInactive()
    {
        if (!InEpisode)
            throw new InvalidOperationException("Episode has already been started.");
    }
    private void AssertEpisodeActive()
    {
        if (!InEpisode)
            throw new InvalidOperationException("Episode hasn't been started yet.");
    }

    private void AssertActionPerformed()
    {
        if (!_lastAction.HasValue)
            throw new InvalidOperationException("Agent didn't perform action on last timestep.");
    }
    #endregion
}
