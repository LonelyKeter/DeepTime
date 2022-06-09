using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Advisor;

using DeepTime.RL.Agents;
using DeepTime.RL;
public static class Agents
{
    public static DQN CreateDQNAgent(IPolicy policy, HyperParameters hyperParameters)
    {
        return new(
            policy,
            new(
                Data.StateConverter.InputSize,
                Data.AdviceEnumerator.EnumCount,
                new() { LearningRate = hyperParameters.LearningRate}),
            hyperParameters);
    }
}
