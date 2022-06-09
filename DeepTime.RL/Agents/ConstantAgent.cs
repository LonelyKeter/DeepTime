using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.RL.Agents;

public class ConstantAgent : IAgent
{
    private static Random _random = new();
    public int StateVecLength { get; }
    public int ActionCount { get; }

    public uint EpisodeNumber { get; private set; } = 0;

    private readonly List<int> _allowedActions = new();
    private bool _shouldFallback = false;
    private readonly int _fallbackAction;

    public ConstantAgent(int stateVecLength, int actionCount, List<int> allowedActions, int fallbackAction)
    {
        StateVecLength = stateVecLength;
        ActionCount = actionCount;
        _allowedActions = allowedActions;
        _fallbackAction = fallbackAction;
    }

    public void EndEpisode(double[] state, double reward) { EpisodeNumber++; }

    public int Eval()
    {
        if (_shouldFallback)
        {
            _shouldFallback = false;
            return _fallbackAction;
        }

        return _allowedActions[_random.Next(0, _allowedActions.Count)];
    }

    public void SetNext(double[] state, double reward)
    {
        if (reward < 0.0)
        {
            _shouldFallback = true;
        }
    }

    public void StartEpisode(double[] state)
    {
    }
}
