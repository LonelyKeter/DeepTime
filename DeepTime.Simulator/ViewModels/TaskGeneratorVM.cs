using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DeepTime.Simulation;

namespace DeepTime.Simulator.ViewModels;

public class TaskGeneratorVM
{
    private TaskGenerator _taskGenerator;

    public TaskGeneratorVM(TaskGenerator taskGenerator)
    {
        VeryHighPriority = new(taskGenerator[Lib.Data.Priority.VeryHigh]);
        HighPriority = new(taskGenerator[Lib.Data.Priority.High]);
        MediumPriority = new(taskGenerator[Lib.Data.Priority.Medium]);
        LowPriority = new(taskGenerator[Lib.Data.Priority.Low]);
        VeryLowPriority = new(taskGenerator[Lib.Data.Priority.VeryLow]);

        _taskGenerator = taskGenerator;
    }

    public int TaskCount { get; set; } = 1;

    public int MinDuration
    {
        get => _taskGenerator.MinDuration;
        set => _taskGenerator.MinDuration = value;
    }

    public int MaxDuration
    {
        get => _taskGenerator.MaxDuration;
        set => _taskGenerator.MaxDuration = value;
    }

    public TaskGenParametersVM VeryHighPriority { get; }
    public TaskGenParametersVM HighPriority { get; }
    public TaskGenParametersVM MediumPriority { get; }
    public TaskGenParametersVM LowPriority { get; }
    public TaskGenParametersVM VeryLowPriority { get; }

    
}
