using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using DeepTime.Simulation;

namespace DeepTime.Simulator.ViewModels;

public class UserVM
{
    public UserConfigVM ConfigVM { get; private set; } = new();
    public UserState State { get; private set; } = new();
}
