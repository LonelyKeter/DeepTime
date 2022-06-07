using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel.DataAnnotations;

using DeepTime.Advisor.Data;

namespace DeepTime.Simulator.ViewModels;

public partial class TaskVM : ObservableObject, ITask
{
    private readonly Task _inner;
    [ObservableProperty]
    private bool _proposed = false;
    [ObservableProperty]
    private string _title = string.Empty;

    public int Id
    {
        get => _inner.Id;
        set => SetProperty(_inner.Id, value, _inner, (m, v) => m.Id = v);
    }
    public Attractiveness Attractiveness
    {
        get => _inner.Attractiveness;
        set => SetProperty(_inner.Attractiveness, value, _inner, (m, v) => m.Attractiveness = v);
    }
    public Priority Priority
    {
        get => _inner.Priority;
        set => SetProperty(_inner.Priority, value, _inner, (m, v) => m.Priority = v);
    }
    public int MinutesEstimate
    {
        get => _inner.MinutesEstimate;
        set => SetProperty(_inner.MinutesEstimate, value, _inner, (m, v) => m.MinutesEstimate = v);
    }
    public int MinutesSpent
    {
        get => _inner.MinutesSpent;
        set => SetProperty(_inner.MinutesSpent, value, _inner, (m, v) => m.MinutesSpent = v);
    }
    public bool Done
    {
        get => _inner.Done;
        set => SetProperty(_inner.Done, value, _inner, (m, v) => m.Done = v);
    }

    public TaskVM(string title, Task inner)
    {
        Title = title;
        _inner = inner;
    }

    public TaskVM(Task inner)
    {
        Title = string.Empty;
        _inner = inner;
    }
}
