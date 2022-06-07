namespace DeepTime.Simulator.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;

using Simulation;

using Simulator.Messages;
using Simulator.Model;
using Simulator.ViewModels;

using Advisor.Data;

using System.IO;
using System.Collections.Generic;
using System.Linq;

public class TaskGeneratorService
{
    private DialogService _dialogService;

    public IMessenger Messenger { get; }

    static readonly DialogService.FormatDescription[] _generatorFormats =
    {
        new("Config", new[] { "*.gcfg" }),
        new("All", new[] { "*.*" }),
    };

    public TaskVMGenerator Generator { get; private set; } = new();

    public TaskGeneratorService(IMessenger messenger, DialogService dialogService)
    {
        Messenger = messenger;
        _dialogService = dialogService;
    }

    public void Load() 
    {
        using var stream = _dialogService.OpenFileStream(_generatorFormats);
        if (stream is null) return;

        Generator = new(TaskGenerator.Deserialize(stream));

        Messenger.Send(new TaskGeneratorLoadedMessage(Generator));
    }

    public void Save()
    {
        using var stream = _dialogService.SaveFileStream(_generatorFormats);
        if (stream is null) return;

        Generator.Inner.Serialize(stream);
    }

    public TaskVM GenerateTask(string title, Priority priority, Attractiveness attractiveness, int minutesEstimate)
    {
        var task = Generator.GenTask(priority, attractiveness, minutesEstimate);
        task.Title = title;

        Messenger.Send(new TasksGeneratedMessage(new[] { task }));
        return task;
    }

    public IReadOnlyCollection<TaskVM> GenerateTasks(int count)
    {
        var tasks = (Generator as ITaskGenerator<TaskVM>).GenTasks(count).ToArray();
        Messenger.Send(new TasksGeneratedMessage(tasks));
        return tasks;
    }

    public IReadOnlyCollection<TaskVM> GenerateDay()
    {
        var tasks = Generator.GenDay().ToArray();

        Messenger.Send(new TasksGeneratedMessage(tasks));
        return tasks;
    }
}
