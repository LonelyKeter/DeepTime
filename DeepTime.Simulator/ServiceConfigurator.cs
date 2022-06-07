namespace DeepTime.Simulator;

using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

using Simulator.Services;
using Simulator.ViewModels;

public static class ServiceConfigurator
{
    public static ServiceProvider CreateAppServiceProvider()
    {
        return new ServiceCollection()
            .AppendServices()
            .AppendViewModels()
        .BuildServiceProvider();
    }

    private static IServiceCollection AppendServices(this IServiceCollection collection)
    {
        return collection
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<DialogService, DialogService>()
            .AddSingleton<AgentService, AgentService>()
            .AddSingleton<SimulationService, SimulationService>()
            .AddSingleton<TaskGeneratorService, TaskGeneratorService>()
            .AddSingleton<TaskManagerService, TaskManagerService>()
            .AddSingleton<UserService, UserService>()
            .AddSingleton<DQNService, DQNService>();
    }

    private static IServiceCollection AppendViewModels(this IServiceCollection collection)
    {
        return collection
            .AddTransient<AddTaskVM>()
            .AddTransient<DoTaskVM>()
            .AddTransient<DQNVM>()
            .AddTransient<SimulationControlVM>()
            .AddTransient<StatisticsVM>()
            .AddTransient<TaskGeneratorVM>()
            .AddTransient<TaskListVM>()
            .AddTransient<UserConfigVM>()
            .AddTransient<UserStateVM>();
    }
}

