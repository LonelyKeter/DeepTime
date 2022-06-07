using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.RL.Aproximators;

[Serializable()]
public class QFunctionApproximator : IQFunction
{
    private readonly NeuralApproximator _model;
    public QApproximatorConfig Config { get; }

    public int ActionCount { get; } 
    public int StateVecLength { get; }

    public QFunctionApproximator(
        int stateVecLength,
        int actionCount,
        QApproximatorConfig config
    )
    {
        _model = new NeuralApproximator(
            stateVecLength,
            actionCount,
            new int[]
            {
                stateVecLength * 3 / 2,
                stateVecLength * 3 / 2,
            },
            1.0,
            0.1
        );

        ActionCount = actionCount;
        StateVecLength = stateVecLength;
        Config = config;
    }   

    public void CorrectQValue(double[] state, int takenAction, double correction)
    {
        var target = _model.Compute(state);
        target[takenAction] = (1 - Config.LearningRate) * target[takenAction] + Config.LearningRate * correction;

        _model.Train(state, target);
    }

    public double[] GetQValues(double[] state)
    {
        return _model.Compute(state);
    }    
}

public class QApproximatorConfig
{
    public const double DefaultLearningRate = 0.001;
    public const int DefaultTrainingStep = 1;

    public int TrainingStep { get; set; } = DefaultTrainingStep;
    public double LearningRate { get; set; } = DefaultLearningRate;
}