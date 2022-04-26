using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepTime.Simulation;

using System.Windows;

namespace DeepTime.Simulator.ViewModels;

public class UserConfigVM : DependencyObject
{
    #region Dependency properties
    public int MaxWorkMinutes
    {
        get => (int)GetValue(MaxWorkMinutesProperty);
        set => SetValue(MaxWorkMinutesProperty, value);
    }
    public static readonly DependencyProperty MaxWorkMinutesProperty =
        DependencyProperty.Register("MaxWorkMinutes", typeof(int), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.MaxWorkMinutes));

    public double Initiativeness
    {
        get => (double)GetValue(InitiativenessProperty);
        set => SetValue(InitiativenessProperty, value);
    }
    public static readonly DependencyProperty InitiativenessProperty =
        DependencyProperty.Register("Initiativeness", typeof(double), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.Initiativeness));

    public int MaxContiniousWorkMinutes
    {
        get => (int)GetValue(MaxContiniousWorkMinutesProperty);
        set => SetValue(MaxContiniousWorkMinutesProperty, value);
    }
    public static readonly DependencyProperty MaxContiniousWorkMinutesProperty =
        DependencyProperty.Register("MaxContiniousWorkMinutes", typeof(int), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.MaxContiniousWorkMinutes));

    public int MaxMinutesOnOneTask
    {
        get => (int)GetValue(MaxMinutesOnOneTaskProperty);
        set => SetValue(MaxMinutesOnOneTaskProperty, value);
    }
    public static readonly DependencyProperty MaxMinutesOnOneTaskProperty =
        DependencyProperty.Register("MaxMinutesOnOneTask", typeof(int), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.MaxMinutesOnOneTask));

    public int MinRest
    {
        get => (int)GetValue(MinRestProperty);
        set => SetValue(MinRestProperty, value);
    }
    public static readonly DependencyProperty MinRestProperty =
        DependencyProperty.Register("MinRest", typeof(int), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.MinRest));

    public double EstimateAccuracy
    {
        get => (double)GetValue(EstimateAccuracyProperty);
        set => SetValue(EstimateAccuracyProperty, value);
    }
    public static readonly DependencyProperty EstimateAccuracyProperty =
        DependencyProperty.Register("EstimateAccuracy", typeof(double), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.EstimateAccuracy));

    public UserStrategy UserStrategy
    {
        get => (UserStrategy)GetValue(UserStrategyProperty);
        set => SetValue(UserStrategyProperty, value);
    }
    public static readonly DependencyProperty UserStrategyProperty =
        DependencyProperty.Register("UserStrategy", typeof(UserStrategy), typeof(UserConfigVM), new PropertyMetadata(UserConfig.Default.Strategy));

    private static UserStrategy[] _userStrategies = Enum.GetValues<UserStrategy>();
    public UserStrategy[] UserStrategies => _userStrategies;    
    #endregion

    public UserConfig? UserConfig = null;
}
