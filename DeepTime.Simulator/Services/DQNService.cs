using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulator.Services;

using DeepTime.Advisor.Data;
using DeepTime.Advisor;
using DeepTime.RL.Policies;
using DeepTime.RL.Agents;

using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Simulation;

using Simulator.Messages;
using Simulator.ViewModels;

public sealed class DQNService
{
    private static readonly DialogService.FormatDescription[] _DQNFormats =
    {
        new("DQN Agent", new[] { "*.dqn"}),
        new("All", new[] {"*.*"})
    };


    private readonly DialogService _dialogService;

    public DQN Agent { get; set; }
    public SoftMax Policy { get; } = new(1.0);

    public IMessenger Messenger { get; }

    public DQNService(DialogService service, IMessenger messenger)
    {
        Agent = Agents.CreateDQNAgent(Policy, new(0.7f, 0.999f));
        Messenger = messenger;
        _dialogService = service;
    }

    public void Load()
    {
        using var stream = _dialogService.OpenFileStream(_DQNFormats);
        if (stream is null) return;

        Agent.LoadFrom(stream);
        Messenger.Send(new DQNLoadedMessage(Agent));
    }

    public void Save()
    {
        using var stream = _dialogService.SaveFileStream(_DQNFormats);
        if (stream is null) return;

        Agent.Serialize(stream);
    }
}
