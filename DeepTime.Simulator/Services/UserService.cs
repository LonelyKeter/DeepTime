namespace DeepTime.Simulator.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.DependencyInjection;

using Simulation;
using ViewModels;
using Messages;

public sealed class UserService
{
    private static readonly DialogService.FormatDescription[] _userConfigFormats =
    {
        new("User config", new[] {"*.ucfg"}),
        new("All", new[] {"*.*"})
    };

    public User<TaskVM> User { get; private set; }
    public IMessenger Messenger { get; }

    public UserService(IMessenger messenger)
    {
        User = new(UserConfig.Default);
        Messenger = messenger;
    }

    public void LoadConfig()
    {
        using var stream = Ioc.Default.GetRequiredService<DialogService>()
            .OpenFileStream(_userConfigFormats);

        if (stream is null) return;
        User.Config.LoadFrom(stream);

        Messenger.Send(new UserConfigLoadedMessage(User.Config));
    }

    public void SaveConfig()
    {
        using var stream = Ioc.Default.GetRequiredService<DialogService>()
            .SaveFileStream(_userConfigFormats);

        if (stream is null) return;
        User.Config.Serialize(stream);
    }
}
