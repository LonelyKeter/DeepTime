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

using static DeepTime.Simulator.ViewModels.ViewModel;

namespace DeepTime.Simulator.ViewModels;

public class DQNVM: DependencyObject
{
    private SoftMax<State> _policy = new(1.0);

    public QApproximatorVM QApproximatorVM { get; } = new();

    public DQN<SoftMax<State>> Agent { get; set; }

    public DQNVM()
    {
        Agent = new(_policy, QApproximatorVM.QFunction, new(0.7f, 0.999f));
    }

    #region Dependency props
    public double SoftMaxTemperature
    {
        get => (double)GetValue(SoftMaxTemperatureProperty);
        set { 
            SetValue(SoftMaxTemperatureProperty, value); 
            _policy.Temperature = value;
        }
    }
    public static readonly DependencyProperty SoftMaxTemperatureProperty =
        DependencyProperty.Register(
            "SoftMaxTemperature",
            typeof(double),
            typeof(DQNVM),
            new PropertyMetadata(1.0)
        );

    public float LearningRate
    {
        get => (float)GetValue(LearningRateProperty);
        set
        {
            SetValue(LearningRateProperty, value);
            Agent.SetLearningRate(value);
        }
    }
    public static readonly DependencyProperty LearningRateProperty =
        DependencyProperty.Register(
            "LearningRate",
            typeof(float),
            typeof(DQNVM),
            new PropertyMetadata(0.7f)
        );



    public float DiscountFactor
    {
        get => (float)GetValue(DiscountFactorProperty);
        set
        {
            SetValue(DiscountFactorProperty, value);
            Agent.SetDiscountFactor(value);
        }
    }
    public static readonly DependencyProperty DiscountFactorProperty =
        DependencyProperty.Register(
            "DiscountFactor",
            typeof(float),
            typeof(DQNVM),
            new PropertyMetadata(0.999f)
        );
    #endregion
}
