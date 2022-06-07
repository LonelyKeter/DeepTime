namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Simulator.Messages;
using Simulator.Services;

using System.Windows.Input;


public partial class DQNVM: ObservableObject,
    IRecipient<DQNLoadedMessage>
{
    private readonly DQNService _DQNService;
    public IMessenger Messenger { get; }

    public DQNVM(DQNService service, IMessenger messenger)
    {
        _DQNService = service;
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    [ICommand]
    public void Load()
    {
        _DQNService.Load();
    }

    [ICommand]
    public void Save()
    {
        _DQNService.Save();
    }

    public void Receive(DQNLoadedMessage message)
    {
        
    }
}
