using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Keras;
using Keras.Models;
using Keras.Layers;
using Keras.Optimizers;

using Numpy;

namespace DeepTime.Lib.Aproximators;

internal class KerasNN { 
    private readonly BaseModel _model;

    private readonly int _inputSize;
    private readonly int _outputSize;

    public KerasNN(int inputSize, int[] hiddenSizes, int outputSize, float learningRate)
    {
        var model = new Sequential();

        model.Add(new Input(new  Shape(inputSize)));
        foreach (var size in hiddenSizes)
        {
            model.Add(new Dense(size, activation: "relu"));
        }
        model.Add(new Dense(outputSize, activation: "linear"));

        model.Compile(optimizer: new Adam(lr: learningRate), loss: "huber");

        _model = model;
    }

    private KerasNN(BaseModel model) => _model = model;

    public void Save(string path)
    {
        _model.Save(path);
    }

    public static KerasNN Load(string path)
    {
        return new(BaseModel.LoadModel(path));
    }

    public float[] Eval(float[] input)
    {
        if (input.Length != _inputSize)
        {
            throw new ArgumentException(
                $"Input of size {input.Length} passed. Expected input of legth {_inputSize}", 
                nameof(input)
            );
        }

        return _model.Predict(np.array(input)).GetData<float>();
    }

    public void Train(float[,] inputs, float[,] targets)
    {
        var sampleCount = ValidateTrain(inputs, targets);

        var x = np.array(inputs);
        var y = np.array(targets);

        _model.Fit(x, y, batch_size: sampleCount / 4 + 1, verbose: 0);
    }

    private int ValidateTrain(float[,] inputs, float[,] targets)
    {
        if (inputs.GetLength(1) != _inputSize)
        {
            throw new ArgumentException(
                $"Inputs of size {inputs.GetLength(1)} passed. Expected inputs of legth {_inputSize}",
                nameof(inputs)
            );
        }

        if (targets.GetLength(1) != _outputSize)
        {
            throw new ArgumentException(
                $"Targets of size {targets.GetLength(1)} passed. Expected inputs of legth {_outputSize}",
                nameof(targets)
            );
        }

        if (inputs.GetLength(0) != targets.GetLength(0))
        {
            throw new ArgumentException(
                $"Different length of Inputs (${inputs.GetLength(0)}) and Targets (${targets.GetLength(0)}) arrays");
        }

        return inputs.GetLength(0);
    }
}
