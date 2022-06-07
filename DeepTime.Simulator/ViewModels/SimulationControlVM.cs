namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Input;

using DeepTime.Simulation;
using Simulator.Services;
using Simulator.Messages;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;


public partial class SimulationControlVM : ObservableValidator,
    IRecipient<DayStartedMessage>,
    IRecipient<DayFinishedMessage>,
    IRecipient<ScheduleContextChangedMessage>
{
    private SimulationService _simulationService;
    private AgentService _agentService;

    public IMessenger Messenger { get; }

    [ObservableProperty]
    [AlsoNotifyChangeFor(nameof(CanChangeAgentType))]
    [AlsoNotifyCanExecuteFor(nameof(StartDayCommand))]
    [AlsoNotifyCanExecuteFor(nameof(FinishDayCommand))]
    [AlsoNotifyCanExecuteFor(nameof(StepForwardCommand))]
    [AlsoNotifyCanExecuteFor(nameof(SimulateDaysCommand))]
    bool _dayGoes;

    public bool CanChangeAgentType => !SimulationGoes;

    [ObservableProperty]
    [AlsoNotifyCanExecuteFor(nameof(StartDayCommand))]
    [AlsoNotifyCanExecuteFor(nameof(FinishDayCommand))]
    [AlsoNotifyCanExecuteFor(nameof(StepForwardCommand))]
    [AlsoNotifyCanExecuteFor(nameof(SimulateDaysCommand))]
    bool _simulationGoes;

    public static readonly string DQNKey = AgentService.DQNKey;
    public IReadOnlyCollection<string> AvailableAgents => AgentService.AvailableAgents;

    #region Observable properties
    public string AgentType
    {
        get => _agentService.CurrentAgent;
        set
        {
            if (value != _agentService.CurrentAgent)
            {
                OnPropertyChanging();
                _agentService.ChangeAgent(value);
                OnPropertyChanged();
            }
        }
    }

    [ObservableProperty]
    [Range(0, int.MaxValue)]
    [AlsoNotifyCanExecuteFor(nameof(SimulateDaysCommand))]
    int _daysToSimulate = 1;

    [ObservableProperty]
    Advisor.Data.ScheduleContext _scheduleContext;

    #endregion

    public SimulationControlVM(
        SimulationService simulationService, 
        AgentService agentService, 
        IMessenger messenger)
    {
        _simulationService = simulationService;
        _agentService = agentService;
        Messenger = messenger;

        _scheduleContext = _simulationService.SimulationTool.ScheduleContext;

        Messenger.RegisterAll(this);
    }

    

    [ICommand(CanExecute = nameof(CanStartDay))]
    public void StartDay()
    {
        if (!CanStartDay) return;

        _simulationService.StartDay();
    }
    bool CanStartDay => !DayGoes && !SimulationGoes;


    [ICommand(CanExecute = nameof(CanFinishDay))]
    public void FinishDay()
    {
        if (!CanFinishDay) return;

        _simulationService.FinishDay();
    }
    bool CanFinishDay => DayGoes && !SimulationGoes;

    [ICommand(CanExecute = nameof(CanStepForward))]
    public void StepForward()
    {
        if (!CanStepForward) return;

        _simulationService.StepForward();
    }
    bool CanStepForward => DayGoes && !SimulationGoes;

    [ICommand(CanExecute = nameof(CanSimulateDays))]
    public async Task SimulateDays()
    {
        if (SimulationGoes)
        {
            SimulationGoes = false;
            return;
        }

        OnSimulationStarted();

        while (SimulationGoes && DaysToSimulate > 0)
        {
            await Task.Run(_simulationService.SimulateDay);
            DaysToSimulate--;
        }

        OnSimulationFinished();
    }
    bool CanSimulateDays => !DayGoes && DaysToSimulate > 0;

    private void OnSimulationStarted()
    {
        SimulationGoes = true;
        Messenger.Send(new SimulationStartedMessage());
    }

    private void OnSimulationFinished()
    {
        SimulationGoes = false;
        Messenger.Send(new SimulationEndedMessage());
    }

    public void Receive(DayStartedMessage message)
    {
        DayGoes = true;
    }

    public void Receive(DayFinishedMessage message)
    {
        DayGoes = false;
    }

    public void Receive(ScheduleContextChangedMessage message)
    {
        ScheduleContext = message.New;
    }
}
