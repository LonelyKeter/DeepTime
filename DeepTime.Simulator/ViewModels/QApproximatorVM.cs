using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using DeepTime.Simulator.Model;
using DeepTime.Simulator.ViewModels;
using DeepTime.Simulator.Commands;

using DeepTime.Lib;
using DeepTime.Lib.Data;
using DeepTime.Lib.Agents;
using DeepTime.Lib.Policies;
using DeepTime.Lib.Aproximators;
using DeepTime.Simulation;

using QFunctionApproximator = DeepTime.Lib.Aproximators.QFunctionApproximator<DeepTime.Lib.Data.State, DeepTime.Lib.Aproximators.StateConverter>;

using static DeepTime.Simulator.ViewModels.ViewModel;

namespace DeepTime.Simulator.ViewModels;

public class QApproximatorVM: DependencyObject
{
    public QFunctionApproximator QFunction { get; private set; } = new(new AdviceEnumerator().EnumCount);

    #region Dependency props
    public int TrainingStep
    {
        get => (int)GetValue(TrainingStepProperty);
        set
        {
            SetValue(TrainingStepProperty, value);
            QFunction.TrainingStep = value;
        }
    } 
    public static readonly DependencyProperty TrainingStepProperty =
        DependencyProperty.Register(
            "TrainingStep",
            typeof(int),
            typeof(QApproximatorVM),
            new PropertyMetadata(QFunctionApproximator.DefaultTrainingStep)
        );


    public float LearningRate
    {
        get => (float)GetValue(LearningRateProperty);
        set
        {
            SetValue(LearningRateProperty, value);
            QFunction.LearningRate = value;
        }
    }
    public static readonly DependencyProperty LearningRateProperty =
        DependencyProperty.Register(
            "LearningRate",
            typeof(float),
            typeof(QApproximatorVM),
            new PropertyMetadata(QFunctionApproximator.DefaultLearningRate)
        );
    #endregion

}
