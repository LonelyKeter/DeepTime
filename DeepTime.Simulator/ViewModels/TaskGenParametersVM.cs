using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeepTime.Lib.Data;

using DeepTime.Simulation;

namespace DeepTime.Simulator.ViewModels;

public class TaskGenParametersVM
{
    public static readonly Attractiveness[] AttractivenessVariants = Enum.GetValues<Attractiveness>();
    
    private readonly GenerationParameters _parameters;

    public TaskGenParametersVM(GenerationParameters parameters)
    {
        _parameters = parameters;
    }

    public int AverageCount { 
        get => _parameters.AverageCount; 
        set => _parameters.AverageCount = value; 
    }
    public int CountDeviation
    {
        get => _parameters.CountDeviation;
        set => _parameters.CountDeviation= value;
    }
    public Attractiveness AverageAttractiveness
    {
        get => _parameters.AverageAttractiveness;
        set => _parameters.AverageAttractiveness= value;
    }
    public int AttractivenessDeviation
    {
        get => _parameters.AttractivenessDeviation;
        set => _parameters.AttractivenessDeviation = value;
    }
}
