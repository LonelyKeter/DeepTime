namespace DeepTime.Simulator.Services;

using Advisor;

using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Simulation;

using DeepTime.Simulator.Messages;
using DeepTime.Simulator.ViewModels;

using System.Linq;

public class SimulationService : 
    IRecipient<TaskGeneratorLoadedMessage>,
    IRecipient<AgentLoadedMessage>
{
    private UserService _userService;
    public IMessenger Messenger { get; }

    public SimulationTool<TaskVM> SimulationTool { get; private set; }
    public int MinQueryMinutes
    {
        get => SimulationTool.MinQueryMinutes;
        set => SimulationTool.MinQueryMinutes = value;
    }

    public SimulationService(
        UserService userService, 
        AgentService agentService, 
        TaskGeneratorService generatorService,
        TaskManagerService managerService,
        IMessenger messenger
        )
    {
        var config = new SimulationConfig<TaskVM>(
            userService.User,
            agentService.Agent,
            generatorService.Generator,
            managerService.Manager,
            //MBDO: Substitute for schedule service data
            new()
            {
                Start = new(8, 0),
                End = new(21, 0)
            },
            AdvisorConfig.Default);

        SimulationTool = new(config);
        Messenger = messenger;
        _userService = userService;

        Messenger.RegisterAll(this);
    }

    public void DoTask(int id, int minutesSpent, int? newEstimate)
    {
        SimulationTool.DoTask(id, minutesSpent, newEstimate);

        UpdateSuggestions();
        SendSimulationStateChangedMessages();
    }

    public void FinishTask(int id, int minutesSpent)
    {
        SimulationTool.FinishTask(id, minutesSpent);

        UpdateSuggestions();
        SendSimulationStateChangedMessages();
    }

    public void Rest()
    {
        SimulationTool.Rest();

        UpdateSuggestions();
        SendSimulationStateChangedMessages();
    }

    public void StepForward()
    {
        SimulationTool.StepForward();

        UpdateSuggestions();
        SendSimulationStateChangedMessages();
    }

    public void SimulateDay()
    {
        var (episodeNumber, entry) = SimulationTool.SimulateDay();
        Messenger.Send(new DayFinishedMessage(episodeNumber, entry));
    }   

    public void StartDay()
    {
        SimulationTool.StartNextDay();

        Messenger.Send(new DayStartedMessage(SimulationTool.TaskManager.ToArray()));
        SendSimulationStateChangedMessages();
    }

    public void FinishDay()
    {
        var (episodeNumber, entry) = SimulationTool.FinishDay();
        Messenger.Send(new DayFinishedMessage(episodeNumber, entry));
    }

    private void UpdateSuggestions()
    {
        foreach (var task in SimulationTool.TaskManager)
        {
            task.Proposed = SimulationTool.LastAdvice.Contains(task);
        }
    }

    #region Recepient implementations
    public void Receive(TaskGeneratorLoadedMessage message)
    {
        SimulationTool.TaskGenerator = message.Generator;
    }

    public void Receive(AgentLoadedMessage message)
    {
        SimulationTool.Agent = message.Agent;
    }
    #endregion

    private void SendSimulationStateChangedMessages()
    {
        Messenger.Send(new NewTaskProposedMessage(SimulationTool.CurrentFeedback));
        Messenger.Send(new ScheduleContextChangedMessage(SimulationTool.ScheduleContext));
        Messenger.Send(new UserStateChangedChangedMessage(_userService.User.State));
    }
}
