using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.RL.Agents;

using DeepTime.RL.Aproximators;

public class ReinforcedGradient : IAgent
{
    private NeuralApproximator _policyApproximator;

    private List<double[]> _stateReplay;
    private List<double[]> _outputReplay;
    private List<double> _rewardReplay; 

    public int StateVecLength => throw new NotImplementedException();

    public int ActionCount => throw new NotImplementedException();

    public void EndEpisode(double[] state, double reward)
    {
        throw new NotImplementedException();
    }

    public int Eval()
    {
        throw new NotImplementedException();
    }

    public void SetNext(double[] state, double reward)
    {
        throw new NotImplementedException();
    }

    public void StartEpisode(double[] state)
    {
        
    }
}
