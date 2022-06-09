using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.RL.Agents;

public class RandomAgent : IAgent
{
    private static Random _random = new();
    public int StateVecLength { get; }
    public int ActionCount { get; }

    public uint EpisodeNumber { get; private set; } = 0;

    public RandomAgent(int stateVecLength, int actionCount)
    {
        StateVecLength = stateVecLength;
        ActionCount = actionCount;
    }

    public void EndEpisode(double[] state, double reward) { EpisodeNumber++; }

    public int Eval()
    {
        return _random.Next(0, ActionCount);
    }

    public void SetNext(double[] state, double reward)
    {
    }

    public void StartEpisode(double[] state)
    {
    }
}
