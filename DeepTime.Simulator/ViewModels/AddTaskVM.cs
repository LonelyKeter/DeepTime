namespace DeepTime.Simulator.ViewModels;

using Simulator.Services;
using Simulator.Messages;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Advisor.Data;

using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using System.Collections.Generic;

public partial class AddTaskVM: ObservableValidator,
    IRecipient<SimulationStartedMessage>,
    IRecipient<SimulationEndedMessage>
{
    private readonly TaskManagerService _managerService;
    private readonly TaskGeneratorService _generatorService;

    #region Observable props
    [ObservableProperty]
    bool _enabled = true;

    [ObservableProperty]
    string _title = string.Empty;

    [ObservableProperty]
    Priority _priority = Priority.Medium;
    public static IReadOnlyList<Priority> PriorityVariants { get; } = Enum.GetValues<Priority>();

    [ObservableProperty]
    Attractiveness _attractiveness = Attractiveness.Medium;
    public static IReadOnlyList<Attractiveness> AttractivenessVariants { get; } = Enum.GetValues<Attractiveness>();

    [ObservableProperty]
    [Range(1, int.MaxValue)]
    int _minutesEstimate = 0;
    #endregion

    public AddTaskVM(TaskManagerService managerService, TaskGeneratorService generatorService)
    {
        _managerService = managerService;
        _generatorService = generatorService;
    }

    [ICommand]
    private void Add()
    {
        var task = _generatorService.GenerateTask(Title, Priority, Attractiveness, MinutesEstimate);
        _managerService.AddTask(task);
    }

    public void Receive(SimulationStartedMessage message)
    {
        Enabled = false;
    }

    public void Receive(SimulationEndedMessage message)
    {
        Enabled = true;
    }
}
