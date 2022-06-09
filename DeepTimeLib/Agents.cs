namespace DeepTime.Advisor;

using DeepTime.Advisor.Data;

using DeepTime.RL.Agents;
using DeepTime.RL;
public static class Agents
{
    public static DQN CreateDQNAgent(IPolicy policy, HyperParameters hyperParameters)
    {
        return new(
            policy,
            new(
                StateConverter.InputSize,
                AdviceEnumerator.EnumCount,
                new() { LearningRate = hyperParameters.LearningRate}),
            hyperParameters);
    }

    public static ConstantAgent CreatePassiveAgent()
        => new(
            StateConverter.InputSize,
            AdviceEnumerator.EnumCount,
            new() { AdviceEnumerator.Enumerate(Advice.PickRest) },
            AdviceEnumerator.Enumerate(Advice.PickRest));

    public static ConstantAgent CreateVeryHighPriorityAgent()
        => new(
            StateConverter.InputSize,
            AdviceEnumerator.EnumCount,
            new(
                Types.AttractivenessesValues
                    .Select(attr => Advice.PickTask(Priority.VeryHigh, attr))
                    .Select(AdviceEnumerator.Enumerate)
                ),
            AdviceEnumerator.Enumerate(Advice.PickRest));

    public static ConstantAgent CreateHighPriorityAgent()
        => new(
            StateConverter.InputSize,
            AdviceEnumerator.EnumCount,
            new(
                Types.AttractivenessesValues
                    .Select(attr => Advice.PickTask(Priority.High, attr))
                    .Select(AdviceEnumerator.Enumerate)
                ),
            AdviceEnumerator.Enumerate(Advice.PickRest));
}
