using System;
using System.Windows;
using System.Windows.Input;

namespace DeepTime.Simulator.ViewModels;

internal static class ViewModel
{
    public static CommandBinding CreateCommandBinding(RoutedCommand command, Action action, Func<bool> canExecute)
        => new(command, (_, _) => action(), (_, args) => args.CanExecute = canExecute());

    public static bool CanAlwaysExecute() => true;
}
