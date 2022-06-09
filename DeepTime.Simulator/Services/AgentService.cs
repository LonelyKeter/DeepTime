namespace DeepTime.Simulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;

using DeepTime.RL;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Simulator.Messages;

public class AgentService
{
    private readonly DQNService _dqnService;

    private readonly IAgent _passive = DeepTime.Advisor.Agents.CreatePassiveAgent();
    private readonly IAgent _naive = DeepTime.Advisor.Agents.CreateVeryHighPriorityAgent();

    public IAgent Agent { get; private set; }
    public IMessenger Messenger { get; }

    public const string DQNKey = "DQN";
    public const string PassiveKey = "Passive";
    public const string NaiveKey = "Naive";

    public static readonly IReadOnlyCollection<string> AvailableAgents = new[] { DQNKey, PassiveKey, NaiveKey };

    public string CurrentAgentType { get; private set; }    

    public AgentService(DQNService dqnService, IMessenger messenger)
    {
        _dqnService = dqnService;

        Messenger = messenger;

        Agent = _dqnService.Agent;
        CurrentAgentType = DQNKey;
    }

    public void ChangeAgent(string type)
    {
        if (!AvailableAgents.Contains(type))
            throw new InvalidOperationException();

        if (string.Equals(CurrentAgentType, type)) return;

        switch (type)
        {
            case DQNKey:
                CurrentAgentType = DQNKey;
                Agent = _dqnService.Agent;
                break;
            case PassiveKey:
                CurrentAgentType = PassiveKey;
                Agent = _passive;
                break;
            case NaiveKey:
                CurrentAgentType = NaiveKey;
                Agent = _naive;
                break;
        }

        Messenger.Send(new AgentLoadedMessage(Agent));
    }
}