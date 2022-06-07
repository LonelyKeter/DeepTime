namespace DeepTime.Simulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DeepTime.RL;
using CommunityToolkit.Mvvm.Messaging;

public class AgentService
{
    private readonly DQNService _dqnService;

    public IAgent Agent { get; private set; }
    public IMessenger Messenger { get; }

    public const string DQNKey = "DQN";
    public static readonly IReadOnlyCollection<string> AvailableAgents = new[] { DQNKey };

    public string CurrentAgent { get; private set; }    

    public AgentService(DQNService dqnService, IMessenger messenger)
    {
        _dqnService = dqnService;

        Messenger = messenger;

        Agent = _dqnService.Agent;
        CurrentAgent = DQNKey;
    }

    public void ChangeAgent(string type)
    {
        if (!AvailableAgents.Contains(type))
            throw new InvalidOperationException();

        switch (type)
        {
            case DQNKey:
                CurrentAgent = DQNKey;
                Agent = _dqnService.Agent;
                break;
        }
    }
}