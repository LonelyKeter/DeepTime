namespace DeepTime.Simulation;

using DeepTime.Advisor.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static DeepTime.Advisor.Data.Types;

using Formatter = System.Runtime.Serialization.Formatters.Binary.BinaryFormatter;

[Serializable()]
public class TaskGenerator : ITaskGenerator<Task>, RL.ISerializable<TaskGenerator>
{
    private static readonly Formatter Formatter = new();
    private static Random _random = new();

    private GenerationParameters[] _parameters = new GenerationParameters[PriorityCount];

    [NonSerialized()] private int _idCounter = 0;

    public int MinDuration { get; set; } = 10;
    public int MaxDuration { get; set; } = 50;

    public bool GeneratesAny() => _parameters.Select(p => p.AverageCount).Any(count => count > 0);

    public GenerationParameters this[Priority priority] => _parameters[priority.AsIndex()];    

    public TaskGenerator()
    {
        foreach (var priority in Enum.GetValues<Priority>())
        {
            _parameters[priority.AsIndex()] = new();
        }
    }

    public TaskGenerator(GenerationParameters[] parameters)
    {
        if (parameters.Length != PriorityCount)
            throw new ArgumentException("Invalid parameters count", nameof(parameters));

        foreach (var priority in Enum.GetValues<Priority>())
        {
            _parameters[priority.AsIndex()] = parameters[priority.AsIndex()];
        }
    }

    public Task GenTask()
    {
        var priority = PickPriority();
        var attractiveness = this[priority].PickAttraciveness(_random);
        return GenTask(priority, attractiveness);
    }

    public Task GenTask(Priority priority, Attractiveness attractiveness) 
        => GenTask(priority, attractiveness, PickDuration());

    public Task GenTask(Priority priority, Attractiveness attractiveness, int minutesEstimate)
        => new(++_idCounter, attractiveness, priority, minutesEstimate, 0, false);

    public IEnumerable<Task> GenDay()
    {
        foreach (var priority in Enum.GetValues<Priority>())
        {
            var count = this[priority].PickCount(_random);
            
            for (var i = 0; i < count; i++) 
                yield return GenTask(priority, this[priority].PickAttraciveness(_random));
        }
    }

    public void ResetCounter() => _idCounter = 0;

    private Priority PickPriority()
    {
        var distribution = new double[PriorityCount];
        var sum = 0.0;

        for (var i = 0; i < PriorityCount; i++)
        {
            distribution[i] = sum + _parameters[i].AverageCount;
            sum += _parameters[i].AverageCount;
        }

        var rand = _random.NextDouble();
        var index = distribution.TakeWhile(val => val / sum < rand).Count();
        
        return PriorityFromIndex(index);
    }

    private int PickDuration()
        => (MinDuration + _random.Next(MaxDuration - MinDuration)) / 5 * 5;

    public static TaskGenerator Deserialize(Stream stream)
    {
        return (TaskGenerator)Formatter.Deserialize(stream);
    }

    public void LoadFrom(Stream stream)
    {
        var instance = Deserialize(stream);

        _parameters = instance._parameters;
        MinDuration = instance.MinDuration;
        MaxDuration = instance.MaxDuration;
    }

    public void Serialize(Stream stream)
    {
        Formatter.Serialize(stream, this);
    }
}


[Serializable()]
public class GenerationParameters
{
    public int AverageCount { get; set; } = 3;
    public int CountDeviation { get; set; } = 1;
    public Attractiveness AverageAttractiveness { get; set; } = Attractiveness.Medium;
    public int AttractivenessDeviation { get; set; } = 1;

    internal int PickCount(Random random)
    {
        var value = AverageCount + random.Next(2 * CountDeviation) - CountDeviation;

        return Math.Max(0, value);
    }

    internal Attractiveness PickAttraciveness(Random random) 
    {
        var value = AverageAttractiveness.AsIndex() + random.Next(2 * AttractivenessDeviation) - AttractivenessDeviation;

        return AttractivenessFromIndex(Math.Clamp(value, Attractiveness.VeryLow.AsIndex(), Attractiveness.VeryHigh.AsIndex()));
    }
}
