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

using Agent = DeepTime.Lib.IAgent<DeepTime.Lib.Data.State, DeepTime.Lib.Data.Advice>;

namespace DeepTime.Simulator.ViewModels;

public class AgentVM: DependencyObject
{
    public Agent Agent { get; } 
}
