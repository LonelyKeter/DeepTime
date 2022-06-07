namespace DeepTime.Simulator.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Simulation;
using Simulator.ViewModels;
using Advisor.Data;

public class TaskVMGenerator : ITaskGenerator<TaskVM>
{
    public TaskGenerator Inner { get; private set; }

    public GenerationParameters this[Priority priority] => Inner[priority];

    public int MinDuration 
    {
        get => Inner.MinDuration;
        set => Inner.MinDuration = value;
    }
    public int MaxDuration
    {
        get => Inner.MaxDuration;
        set => Inner.MaxDuration = value;
    }

    public TaskVMGenerator()
    {
        Inner = new();
    }

    public TaskVMGenerator(GenerationParameters[] parameters)
    {
        Inner = new(parameters);
    }

    public TaskVMGenerator(TaskGenerator generator)
    {
        Inner = generator;
    }

    public IEnumerable<TaskVM> GenDay()
        => Inner.GenDay().Select(task => new TaskVM(task));

    public bool GeneratesAny()
        => Inner.GeneratesAny();

    public TaskVM GenTask()
        => new(Inner.GenTask());

    public TaskVM GenTask(Priority priority, Attractiveness attractiveness)
        => new(Inner.GenTask(priority, attractiveness));

    public TaskVM GenTask(Priority priority, Attractiveness attractiveness, int minutesEstimate)
        => new(Inner.GenTask(priority, attractiveness, minutesEstimate));

    public void ResetCounter()
        => Inner.ResetCounter();
}
