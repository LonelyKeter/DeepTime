namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Simulation;

using Simulator.Messages;
using Simulator.Services;

public partial class UserStateVM : ObservableObject,
    IRecipient<UserStateChangedChangedMessage>
{
    private readonly UserService _userService;

    public IMessenger Messenger { get; }

    #region Observable props
    [ObservableProperty]
    UserState _state;
    #endregion

    public UserStateVM(UserService service, IMessenger messenger)
    {
        _userService = service;
        State = _userService.User.State;
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    public void Receive(UserStateChangedChangedMessage message)
    {
        State = message.New;
    }
}
