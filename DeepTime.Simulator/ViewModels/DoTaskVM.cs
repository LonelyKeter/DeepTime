namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;

using Simulator.Messages;
using Simulator.Services;

public partial class DoTaskVM : ObservableValidator, 
    IRecipient<NewTaskProposedMessage>,
    IRecipient<TaskSelectedMessage>,
    IRecipient<DayStartedMessage>,
    IRecipient<DayFinishedMessage>,
    IRecipient<SimulationStartedMessage>,
    IRecipient<SimulationEndedMessage>
{
    private readonly SimulationService _simulationService;
    public IMessenger Messenger { get; private set; }

    #region Observable props
    [ObservableProperty]
    bool _enabled;

    [ObservableProperty]
    [Range(0, int.MaxValue)]
    private int _minutesSpent;

    [ObservableProperty]
    [Range(0, int.MaxValue)]
    private int? _newEstimate;

    [ObservableProperty]
    private bool _finished = false;

    [ObservableProperty]
    [AlsoNotifyCanExecuteFor(nameof(DoTaskCommand))]
    private TaskVM? _selectedTask = null;

    [ObservableProperty]
    bool _available;
    #endregion

    public DoTaskVM(SimulationService simulationService, IMessenger messenger)
    {
        _simulationService = simulationService;
        _available = _simulationService.SimulationTool.DayGoes;

        Messenger = messenger;
        Messenger.RegisterAll(this);
    }

    [ICommand(CanExecute = nameof(DoTaskCanExecute))]
    private void DoTask()
    {
        if (Finished)
        {
            _simulationService.FinishTask(SelectedTask.Id, MinutesSpent);
        }
        else
        {
            _simulationService.DoTask(SelectedTask.Id, MinutesSpent, NewEstimate);
        }
    }
    private bool DoTaskCanExecute => SelectedTask is not null;

    [ICommand]
    private void Rest()
    {
        _simulationService.Rest();
    }

    public void Receive(NewTaskProposedMessage message)
    {
        var feedback = message.Feedback;

        if (feedback.HasValue)
        {
            var value = feedback.Value;

            MinutesSpent = value.MinutesSpent;
            NewEstimate = value.NewEstimate;
            Finished = value.Done;            
        }
        else
        {
            MinutesSpent = 0;
            NewEstimate = null;
            Finished = false;
        }
    }

    public void Receive(TaskSelectedMessage message)
    {
        SelectedTask = message.Task;
    }

    public void Receive(DayStartedMessage message)
    {
        Available = true;
    }

    public void Receive(DayFinishedMessage message)
    {
        Available = false;
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

