
using AForge.Neuro;
using AForge.Neuro.Learning;

namespace DeepTime.RL.Aproximators;

[Serializable()]
internal class NeuralApproximator
{
    private ActivationNetwork _network;
    private BackPropagationLearning _teacher;

    public NeuralApproximator(int inputSize, int outputSize, IEnumerable<int> hiddenSizes, double learningRate, double momentum)
    {
        var layerSizes = new List<int>();
        layerSizes.AddRange(hiddenSizes);
        layerSizes.Add(outputSize);

        _network = new(new SigmoidFunction(), inputSize, layerSizes.ToArray());
        _teacher = new(_network) 
        { 
            LearningRate = learningRate, 
            Momentum = momentum 
        };
    }

    public double[] Compute(double[] input)
    {
        return _network.Compute(input);
    }

    public double Train(double[] input, double[] target)
    {
        return _teacher.Run(input, target);
    }
    public double Train(double[][] inputs, double[][] targets)
    {
        return _teacher.RunEpoch(inputs, targets);
    }
}
