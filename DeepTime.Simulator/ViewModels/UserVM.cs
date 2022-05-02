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

using DeepTime.Simulation;

using static DeepTime.Simulator.ViewModels.ViewModel;

namespace DeepTime.Simulator.ViewModels;

public class UserVM : DependencyObject
{
    public User User { get; private set; }

    public UserConfigVM ConfigVM { get; private set; } = new();

    #region Dependency props
    public UserState State
    {
        get => (UserState)GetValue(StateProperty);
        set => SetValue(StateProperty, value);
    }
    public static readonly DependencyProperty StateProperty =
        DependencyProperty.Register(
            "State",
            typeof(UserState),
            typeof(UserVM),
            new PropertyMetadata()
        );    
    #endregion

    public UserVM()
    {
        User = new(ConfigVM.UserConfig);
        State = User.State;
    }

    public void UpdateState()
    {
        State = User.State;
    }
}
