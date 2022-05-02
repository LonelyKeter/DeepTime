using DeepTime.Lib.Aproximators;
using DeepTime.Lib.Data;

using System.Text.Json.Serialization;
using System.Text.Json;

namespace DeepTime.Lib.Agents;

public class DQN<TPolicy> : IAgent<State, Advice>
    where TPolicy : IPolicy<State>
{
    private HyperParameters _hyperParameters;

    private readonly TPolicy _policy;
    private readonly QFunctionApproximator<State, StateConverter> _qApproximator;
    private readonly AdviceEnumerator _adviceEnumerator = new();

    public HyperParameters HyperParameters => _hyperParameters;

    public void SetLearningRate(double rate) => _hyperParameters.LearningRate = rate;
    public void SetDiscountFactor(double factor) => _hyperParameters.DiscountFactor = factor;

    

    private bool InEpisode => _lastState.HasValue;

    private Advice? _lastAdvice;
    private State? _lastState;

    public DQN(TPolicy policy, QFunctionApproximator<State, StateConverter> qApproximator, HyperParameters hyperParameters)
    {
        _policy = policy;
        _qApproximator = qApproximator;
        _hyperParameters = hyperParameters;
    }



    public void EndEpisode(State state, double reward)
    {
        if (!InEpisode)
            throw new InvalidOperationException("Episode wasn't started.");

        CorrectQValue(state, reward, true);

        _lastAdvice = null;
        _lastState = null;
    }

    public void StartEpisode(State state)
    {
        _lastState = state;
    }

    public Advice Eval()
    {
        AssertEpisodeActive();

        var encoded = _policy.Eval(_lastState.Value, _qApproximator);
        return _adviceEnumerator[encoded];
    }

    public void SetNext(State state, double reward)
    {
        AssertEpisodeActive();

        CorrectQValue(state, reward);
        _lastState = state;
    }

    private void CorrectQValue(State newState, double reward, bool terminal = false)
    {
        if (_lastAdvice.HasValue)
        {
            var advice = _lastAdvice.Value;

            var correction = terminal ? reward : reward + _hyperParameters.DiscountFactor * _qApproximator.GetQValues(newState).Max();
            _qApproximator.CorrectQValue(_lastState.Value, _adviceEnumerator[advice], correction);
        }        
    }

    public string ToJson()
    {
        throw new NotImplementedException();
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
        if (!_lastAdvice.HasValue)
            throw new InvalidOperationException("Agent didn't perform action on last timestep.");
    }
    #endregion
}
