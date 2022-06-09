namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Simulator.Messages;
using Simulator.Services;

using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

public sealed partial class TaskListVM: ObservableObject,
    IRecipient<TasksAddedMessage>,
    IRecipient<TaskDeletedMessage>,
    IRecipient<TasksClearedMessage>,
    IRecipient<DayStartedMessage>,
    IRecipient<NewTaskProposedMessage>,
    IRecipient<SimulationStartedMessage>,
    IRecipient<SimulationEndedMessage>
{
    private TaskManagerService _managerService;
    private ObservableCollection<TaskVM> _tasks = new();
    public ObservableCollection<TaskVM> Tasks
    {
        get => _tasks;
        private set => SetProperty(ref _tasks, value);
    }

    public IMessenger Messenger { get; }

    #region Observable props 
    [ObservableProperty]
    bool _enabled = true;


    private TaskVM? _selectedTask = null;
    public TaskVM? SelectedTask
    {
        get => _selectedTask;
        set
        {
            if (SetProperty(ref _selectedTask, value))
            {
                Messenger.Send(new TaskSelectedMessage(value));
                NotifyModificationCommandsCanExecuteChanged();
            }
        }
    }
    #endregion

    public TaskListVM(TaskManagerService service, IMessenger messenger)
    {
        _managerService = service;
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    [ICommand(CanExecute = nameof(DeleteCanExecute))]
    public void Delete()
    {
        if (SelectedTask is null) return;

        _managerService.DeleteTask(SelectedTask.Id);
    }
    private bool DeleteCanExecute
        => SelectedTask is not null;

    [ICommand(CanExecute = nameof(ClearCanExecute))]
    public void Clear()
    {
        if (Tasks.Count == 0) return;

        _managerService.Clear();
    }
    private bool ClearCanExecute => Tasks.Count > 0;

    private void NotifyModificationCommandsCanExecuteChanged()
    {
        ClearCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    #region IRecipient implemetations
    public void Receive(TasksClearedMessage message)
    {
        Tasks.Clear();
        NotifyModificationCommandsCanExecuteChanged();
    }

    public void Receive(TasksAddedMessage message)
    {
        foreach (var task in message.Tasks)
        {
            Tasks.Add(task);
        }

        NotifyModificationCommandsCanExecuteChanged();
    }

    public void Receive(DayStartedMessage message)
    {
        Tasks.Clear();
        foreach (var task in message.Tasks)
        {
            Tasks.Add(task);
        }

        NotifyModificationCommandsCanExecuteChanged();
    }

    public void Receive(NewTaskProposedMessage message)
    {
        var feedback = message.Feedback;

        if (feedback.HasValue)
        {
            var id = feedback.Value.TaskId;

            SelectedTask = Tasks.First(task => task.Id == id);
        }
        else
        {
            SelectedTask = null;
        }

        NotifyModificationCommandsCanExecuteChanged();
    }

    public void Receive(TaskDeletedMessage message)
    {
        Tasks.Remove(message.Task);

        NotifyModificationCommandsCanExecuteChanged();
    }

    public void Receive(SimulationStartedMessage message)
    {
        Enabled = false;
    }

    public void Receive(SimulationEndedMessage message)
    {
        Enabled = true;
    }
    #endregion
}
