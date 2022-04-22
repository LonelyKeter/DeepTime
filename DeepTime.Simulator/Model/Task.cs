using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using DeepTime.Lib;

namespace DeepTime.Simulator.Model;

internal class Task : INotifyPropertyChanged
{
    private bool _proposed = false;

    public string Title { get; init; }
    public bool Proposed
    {
        get => _proposed;
        set
        {
            _proposed = value;
            OnPropertyChanged();
        }
    }

    public Task(string title, Lib.Data.Task inner)
    {
        Title = title;
        Inner = inner;
    }

    public Lib.Data.Task Inner { get; init; }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
