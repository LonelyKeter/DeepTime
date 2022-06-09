namespace DeepTime.Simulator;

using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

using Simulator.Services;
using Simulator.ViewModels;


using LiveChartsCore;
/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Ioc.Default.ConfigureServices(
            ServiceConfigurator.CreateAppServiceProvider()
        );
    }
}
