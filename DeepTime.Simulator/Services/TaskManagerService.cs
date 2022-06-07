namespace DeepTime.Simulator.Services;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Advisor;
using DeepTime.Advisor.Data;

using Simulator.Messages;
using Simulator.ViewModels;

using System.Linq;
using System.Collections.Generic;

public class TaskManagerService
{
    public IMessenger Messenger { get; }
    public TaskManager<TaskVM> Manager { get; } = new();

    public TaskManagerService(IMessenger messenger)
    {
        Messenger = messenger;

        Messenger.RegisterAll(this);
    }

    public void AddTask(TaskVM task)
    {
        Manager.Add(task);
        Messenger.Send(new TasksAddedMessage(new[] { task }));
    }

    public void AddTasks(IEnumerable<TaskVM> tasks)
    {
        var list = tasks.ToArray();

        foreach (var task in list)
        {
            Manager.Add(task);
        }

        Messenger.Send(new TasksAddedMessage(list));
    }

    public void DeleteTask(int id)
    {
        Messenger.Send(new TaskDeletedMessage(Manager.Remove(id)));
    }

    public void Clear()
    {
        Messenger.Send(new TasksClearedMessage(Manager.Clear().ToArray()));
    }
}
