using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Keras;

namespace DeepTime.Lib.Aproximators;


public class QFunctionApproximator<TState, TStateConverter> : IQFunction<TState>
    where TStateConverter : IStateConverter<TState>
{
    private readonly KerasNN _model;
    private readonly TStateConverter _stateConverter;
    private readonly List<ReplayEntry> _replay = new();

    private readonly int _trainingStep;
    private readonly float _learningRate;

    public int ActionCount { get; init; } 

    public QFunctionApproximator(
        TStateConverter stateConverter,
        int actionCount,
        float learningRate = 0.001f, 
        int trainingStep = 1
    )
    {
        _model = new KerasNN(
            stateConverter.InputSize,
            new int[] { stateConverter.InputSize * 2 / 3, stateConverter.InputSize * 2 / 3 },
            actionCount,
            learningRate
        );

        ActionCount = actionCount;
        _stateConverter = stateConverter;

        _trainingStep = trainingStep;
        _learningRate = learningRate;
    }

    public void CorrectQValue(TState state, int takenAction, float correction)
    {
        AddReplay(state, takenAction, correction);

        if (_replay.Count >= _trainingStep)
        {
            Train();
        }
    }

    public float[] GetQValues(TState state)
    {
        var input = _stateConverter.ToInput(state);
        return _model.Eval(input);
    }   

    private void AddReplay(TState state, int takenAction, float reward)
    {
        if (takenAction >= ActionCount)
            throw new ArgumentOutOfRangeException(nameof(takenAction));

        _replay.Add(new(
            _stateConverter.ToInput(state),
            takenAction,
            reward
        ));
    }

    private (float[], float[]) ConstructSample(ReplayEntry entry)
    {
        var target = _model.Eval(entry.OldState);
        target[entry.TakenAction] = (1 - _learningRate) * target[entry.TakenAction] + _learningRate * entry.Correction;

        return (entry.OldState, target);
    } 

    private void Train()
    {
        var inputs = new float[_replay.Count, _stateConverter.InputSize];
        var targets = new float[_replay.Count, ActionCount];

        for (var index = 0; index < _replay.Count; index++)
        {
            var entry = _replay[index];

            var (input, target) = ConstructSample(entry);

            CopyOneDToTwoD(input, inputs, index);
            CopyOneDToTwoD(target, targets, index);
        }

        _model.Train(inputs, targets);
    }

    private static void CopyOneDToTwoD<T>(T[] source, T[,] dist, int distOffset)
    {
        for (var index = 0; index < source.Length; index++)
        {
            dist[distOffset, index] = source[index];
        }
    }
    
    private record struct ReplayEntry(float[] OldState, int TakenAction, float Correction);
}
