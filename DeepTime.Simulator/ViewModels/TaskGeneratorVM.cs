namespace DeepTime.Simulator.ViewModels;

using Advisor.Data;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Simulator.Model;
using Simulator.Messages;
using Simulator.Services;

using System;
using System.ComponentModel.DataAnnotations;

using Validation;

public partial class TaskGeneratorVM : ObservableValidator,
    IRecipient<TaskGeneratorLoadedMessage>,
    IRecipient<SimulationStartedMessage>,
    IRecipient<SimulationEndedMessage>
{
    private readonly TaskGeneratorService _generatorService;
    private readonly Services.TaskManagerService _managerService;

    public IMessenger Messenger { get; }

    public TaskGeneratorVM(
        TaskGeneratorService generatorService,
        Services.TaskManagerService managerService, 
        IMessenger messenger)
    {
        _generatorService = generatorService;
        _managerService = managerService;

        var generator = generatorService.Generator;
        _veryHighPriority = new(generator[Priority.VeryHigh]);
        _highPriority = new(generator[Priority.High]);
        _mediumPriority = new(generator[Priority.Medium]);
        _lowPriority = new(generator[Priority.Low]);
        _veryLowPriority = new(generator[Priority.VeryLow]);

        Messenger = messenger;
        Messenger.RegisterAll(this);
    }

    [ObservableProperty]
    bool _enabled = true;

    [ObservableProperty]
    [Required]
    [Range(0, int.MaxValue)]
    int _taskCount = 1;

    [Required]
    [Range(0.0, double.MaxValue)]
    [LessThan(nameof(MaxDuration))]
    public int MinDuration
    {
        get => _generatorService.Generator.MinDuration;
        set => SetProperty(_generatorService.Generator.MinDuration, value, _generatorService.Generator, (m, v) => m.MinDuration = v, true);
    }

    [Required]
    [Range(0.0, double.MaxValue)]
    [GreaterThan(nameof(MinDuration))]
    public int MaxDuration
    {
        get => _generatorService.Generator.MaxDuration;
        set => SetProperty(_generatorService.Generator.MaxDuration, value, _generatorService.Generator, (m, v) => m.MaxDuration = v);
    }

    [ObservableProperty]
    private TaskGenParametersVM _veryHighPriority;
    [ObservableProperty]
    private TaskGenParametersVM _highPriority;
    [ObservableProperty]
    private TaskGenParametersVM _mediumPriority;
    [ObservableProperty]
    private TaskGenParametersVM _lowPriority;
    [ObservableProperty]
    private TaskGenParametersVM _veryLowPriority;  

    [ICommand]
    public void Load()
    {
        _generatorService.Load();
    }

    [ICommand]
    public void Save()
    {
        _generatorService.Save();
    }
    

    [ICommand]
    public void GenerateDay()
    {
        var tasks = _generatorService.GenerateDay();
        _managerService.Clear();
        _managerService.AddTasks(tasks);
    }

    [ICommand]
    public void GenerateTasks()
    {
        var tasks = _generatorService.GenerateTasks(_taskCount);
        _managerService.AddTasks(tasks);
    }

    public void Receive(TaskGeneratorLoadedMessage message)
    {
        var generator = message.Generator;

        VeryHighPriority = new(generator[Priority.VeryHigh]);
        HighPriority = new(generator[Priority.High]);
        MediumPriority = new(generator[Priority.Medium]);
        LowPriority = new(generator[Priority.Low]);
        VeryLowPriority = new(generator[Priority.VeryLow]);
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
