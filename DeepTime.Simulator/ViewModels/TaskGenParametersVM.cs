using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeepTime.Lib.Data;

namespace DeepTime.Simulator.ViewModels;

public class TaskGenParametersVM
{
    private static Attractiveness[] _attractivenessVariants = Enum.GetValues<Attractiveness>();
    public Attractiveness[] AttractivenessVariants => _attractivenessVariants;

    public int AverageCount { get; set; } = 0;
    public int CountDeviation { get; set; } = 0;
    public Attractiveness AverageAttractiveness { get; set; } = Attractiveness.Medium;
    public int AttractivenessDeviation { get; set; } = 0;
}
