namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Simulation;
using Simulator.Services;
using Simulator.Messages;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


public sealed partial class UserConfigVM : ObservableValidator,
    IRecipient<UserConfigLoadedMessage>,
    IRecipient<SimulationStartedMessage>,
    IRecipient<SimulationEndedMessage>
{
    public IReadOnlyList<UserStrategy> UserStrategies { get; } = Enum.GetValues<UserStrategy>();

    private UserService _userService;
    private UserConfig _inner;

    public IMessenger Messenger { get; }


    public UserConfigVM(UserService service, IMessenger messenger)
    {
        _userService = service;
        _inner = service.User.Config;
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    #region Observable properties
    [ObservableProperty]
    bool _enabled = true;

    [Required]
    [Range(1, int.MaxValue)] 
    public int MaxWorkMinutes 
    { 
        get => _inner.MaxWorkMinutes; 
        set => SetProperty(_inner.MaxWorkMinutes, value, _inner, (m, v) => m.MaxWorkMinutes = v, true);
    }

    [Required]
    [Range(0.0, 1.0)]
    public double Initiativeness
    {
        get => _inner.Initiativeness;
        set => SetProperty(_inner.Initiativeness, value, _inner, (m, v) => m.Initiativeness = v, true);
    }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxContiniousWorkMinutes
    {
        get => _inner.MaxContiniousWorkMinutes;
        set => SetProperty(_inner.MaxContiniousWorkMinutes, value, _inner, (m, v) => m.MaxContiniousWorkMinutes = v, true);
    }

    [Required]
    [Range(1, int.MaxValue)]
    public int MaxMinutesOnOneTask
    {
        get => _inner.MaxMinutesOnOneTask;
        set => SetProperty(_inner.MaxMinutesOnOneTask, value, _inner, (m, v) => m.MaxMinutesOnOneTask = v, true);
    }

    [Required]
    [Range(1, int.MaxValue)]
    public int MinRest
    {
        get => _inner.MinRest;
        set => SetProperty(_inner.MinRest, value, _inner, (m, v) => m.MinRest = v, true);
    }

    [Required]
    [Range(0.0, 1.0)]
    public double EstimateAccuracy
    {
        get => _inner.EstimateAccuracy;
        set => SetProperty(_inner.EstimateAccuracy, value, _inner, (m, v) => m.EstimateAccuracy = v);
    }

    [Required]
    public UserStrategy Strategy
    {
        get => _inner.Strategy;
        set => SetProperty(_inner.Strategy, value, _inner, (m, v) => m.Strategy = v);
    }
    #endregion

    [ICommand]
    public void Load()
    {
        _userService.LoadConfig();
    }

    [ICommand]
    public void Save()
    {
        _userService.SaveConfig();
    }

    public void Receive(UserConfigLoadedMessage message)
    {
        _inner = message.Config;
        OnPropertyChanged(string.Empty);
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
