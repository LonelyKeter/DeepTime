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

using static DeepTime.Simulator.ViewModels.ViewModel;

namespace DeepTime.Simulator;

internal class MainVM : DependencyObject
{
    public TaskVM TaskVM => SimulationVM.TaskVM;
    public UserVM UserVM => SimulationVM.UserVM;
    public SimulationVM SimulationVM { get; } = new();

    public IList CreateCommandBindings() => new object[]
    {
    };    
}
