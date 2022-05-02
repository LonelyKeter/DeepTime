using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Lib.Aproximators;


public class QFunctionApproximator<TState, TStateConverter> : IQFunction<TState>
    where TStateConverter : IStateConverter<TState>, new()
{
    public const double DefaultLearningRate = 0.001;
    public const int DefaultTrainingStep = 1;

    private readonly NeuralApproximator _model;
    private readonly TStateConverter _stateConverter = new();
    private readonly List<ReplayEntry> _replay = new();

    public int TrainingStep { get; set; }
    public double LearningRate { get; set; }

    public int ActionCount { get; init; } 

    public QFunctionApproximator(
        int actionCount,
        double learningRate = DefaultLearningRate, 
        int trainingStep = DefaultTrainingStep
    )
    {
        var inputSize = _stateConverter.InputSize;

        _model = new NeuralApproximator(
            inputSize,
            actionCount,
            new int[]
            {
                inputSize * 3 / 2,
                inputSize * 3 / 2,
            },
            learningRate,
            0.1);

        ActionCount = actionCount;

        TrainingStep = trainingStep;
        LearningRate = learningRate;
    }   

    public void CorrectQValue(TState state, int takenAction, double correction)
    {
        AddReplay(state, takenAction, correction);

        if (_replay.Count >= TrainingStep)
        {
            Train();
        }
    }

    public double[] GetQValues(TState state)
    {
        var input = _stateConverter.ToInput(state);
        return _model.Compute(input);
    }   

    private void AddReplay(TState state, int takenAction, double reward)
    {
        if (takenAction >= ActionCount)
            throw new ArgumentOutOfRangeException(nameof(takenAction));

        _replay.Add(new(
            _stateConverter.ToInput(state),
            takenAction,
            reward
        ));
    }

    private (double[], double[]) ConstructSample(ReplayEntry entry)
    {
        var target = _model.Compute(entry.OldState);
        target[entry.TakenAction] = (1 - LearningRate) * target[entry.TakenAction] + LearningRate * entry.Correction;

        return (entry.OldState, target);
    } 

    private void Train()
    {
        var inputs = new double[_replay.Count][];
        var targets = new double[_replay.Count][];

        for (var index = 0; index < _replay.Count; index++)
        {
            var entry = _replay[index];
            var (input, target) = ConstructSample(entry);
            inputs[index] = input;
            targets[index] = target;
        }

        _model.Train(inputs, targets);
    }
    
    private record struct ReplayEntry(double[] OldState, int TakenAction, double Correction);
}
