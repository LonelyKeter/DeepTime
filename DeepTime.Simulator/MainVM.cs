using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

using DeepTime.Simulator.Model;
using DeepTime.Simulator.ViewModels;
using DeepTime.Lib;

namespace DeepTime.Simulator;

internal class MainVM : DependencyObject
{
    private int _lastTaskId = 0;

    public TaskVM TaskVM { get; } = new();
    public UserVM UserVM { get; } = new();

    public ScheduleContextModel ScheduleContext
    {
        get => (ScheduleContextModel)GetValue(ScheduleContextProperty);
        set => SetValue(ScheduleContextProperty, value);
    }
    public static readonly DependencyProperty ScheduleContextProperty =
        DependencyProperty.Register("ScheduleContext", typeof(ScheduleContextModel), typeof(MainVM), new PropertyMetadata(null));

    public IList CreateCommandBindings() => new[]
    {
        new CommandBinding(TestCommands.Rest, Rest, RestCanExecute),
        new CommandBinding(TestCommands.DoTask, DoTask, DoTaskCanExecute),
        new CommandBinding(TestCommands.FinishTask, FinishTask, FinishTaskCanExecute),
        new CommandBinding(TestCommands.AddTask, AddTask, AddTaskCanExecute),
        new CommandBinding(TestCommands.StartDay, StartDay, StartDayCanExecute),
        new CommandBinding(TestCommands.FinishDay, FinishDay, FinishDayCanExecute),
    };

    private void Rest(object sender, ExecutedRoutedEventArgs args)
    {

    }

    private void RestCanExecute(object sender, CanExecuteRoutedEventArgs args) 
        => args.CanExecute = true;

    private void DoTask(object sender, ExecutedRoutedEventArgs args)
    {

    }

    private void DoTaskCanExecute(object sender, CanExecuteRoutedEventArgs args)
        => args.CanExecute = true;

    private void FinishTask(object sender, ExecutedRoutedEventArgs args)
    {

    }

    private void FinishTaskCanExecute(object sender, CanExecuteRoutedEventArgs args)
        => args.CanExecute = true;

    private void AddTask(object sender, ExecutedRoutedEventArgs args)
    {
        var newTask = TaskVM.GetInputTask(++_lastTaskId);
        TaskVM.Tasks.Add(newTask);
    }

    private void AddTaskCanExecute(object sender, CanExecuteRoutedEventArgs args)
        => args.CanExecute = true;

    private void StartDay(object sender, ExecutedRoutedEventArgs args)
    {

    }

    private void StartDayCanExecute(object sender, CanExecuteRoutedEventArgs args)
        => args.CanExecute = true;

    private void FinishDay(object sender, ExecutedRoutedEventArgs args)
    {

    }

    private void FinishDayCanExecute(object sender, CanExecuteRoutedEventArgs args)
        => args.CanExecute = true;
}
