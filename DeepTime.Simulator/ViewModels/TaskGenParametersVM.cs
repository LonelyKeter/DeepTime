namespace DeepTime.Simulator.ViewModels;

using Advisor.Data;

using CommunityToolkit.Mvvm.ComponentModel;

using Simulation;

using System;
using System.ComponentModel.DataAnnotations;


public class TaskGenParametersVM : ObservableObject
{
    public static readonly Attractiveness[] AttractivenessVariants = Enum.GetValues<Attractiveness>();

    private readonly GenerationParameters _parameters;

    public TaskGenParametersVM(GenerationParameters parameters)
    {
        _parameters = parameters;
    }

    [Required]
    [Range(0, int.MaxValue)]
    public int AverageCount
    {
        get => _parameters.AverageCount;
        set => SetProperty(_parameters.AverageCount, value, _parameters, (m, v) => m.AverageCount = v);
    }

    [Required]
    [Range(0, int.MaxValue)]
    public int CountDeviation
    {
        get => _parameters.CountDeviation;
        set => SetProperty(_parameters.CountDeviation, value, _parameters, (m, v) => m.CountDeviation = v);
    }

    [Required]
    public Attractiveness AverageAttractiveness
    {
        get => _parameters.AverageAttractiveness;
        set => SetProperty(_parameters.AverageAttractiveness, value, _parameters, (m, v) => m.AverageAttractiveness = v);
    }

    [Required]
    [Range(0, int.MaxValue)]
    public int AttractivenessDeviation
    {
        get => _parameters.AttractivenessDeviation;
        set => SetProperty(_parameters.AttractivenessDeviation, value, _parameters, (m, v) => m.AttractivenessDeviation = v);
    }
}
