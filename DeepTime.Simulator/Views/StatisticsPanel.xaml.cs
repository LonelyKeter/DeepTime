using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DeepTime.Simulator.Views;

using CommunityToolkit.Mvvm.DependencyInjection;

using DeepTime.Simulator.ViewModels;

/// <summary>
/// Логика взаимодействия для StatisticsPanel.xaml
/// </summary>
public partial class StatisticsPanel : UserControl
{
    public StatisticsPanel()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<StatisticsVM>();
    }
}
