using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Simulator.ViewModels;

using DeepTime.Simulator.Services;
using DeepTime.Simulator.Messages;

using DeepTime.Advisor.Statistics;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using System.Collections.ObjectModel;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

public sealed partial class StatisticsVM : ObservableObject,
    IRecipient<DayFinishedMessage>
{
    private readonly ObservableCollection<StatisticsEntry> _entries = new();
    private readonly ObservableCollection<double> _medians = new();
    private readonly ObservableCollection<double> _averages = new();


    public ISeries[] Medians { get; }
    public Axis[] MediansXAxis { get; } = new[]
    {
        new Axis
        {
            MinStep = 1,
        }
    };
    public Axis[] MediansYAxis { get; } = new[]
    {
        new Axis
        {
            MinStep = 1,
        }
    };

    public ReadOnlyObservableCollection<StatisticsEntry> Entries => new(_entries);

    private readonly IStatistics _inner;

    public IMessenger Messenger { get; }

    #region Observable props 
    [ObservableProperty]
    double? _min;
    [ObservableProperty]
    double? _max;
    [ObservableProperty]
    double? _median;
    [ObservableProperty]
    double? _average;
    #endregion

    public StatisticsVM(SimulationService simulationService, IMessenger messenger)
    {
        _inner = simulationService.SimulationTool.Statistics;
        Messenger = messenger;

        Medians = new ISeries[]
        {
            CreateMedianSeries(_medians),
            CreateAverageSeries(_averages),
        };

        Messenger.RegisterAll(this);
    }

    [ICommand(CanExecute = nameof(CanClear))]
    public void Clear()
    {
        _inner.Clear();
        _entries.Clear();
        _medians.Clear();
        _averages.Clear();

        Min = null;
        Max = null;
        Median = null;
        Average = null;

        ClearCommand.NotifyCanExecuteChanged();
    }
    bool CanClear => _inner.Count > 0;

    public void Receive(DayFinishedMessage message)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _entries.Add(message.Entry);

            Min = _inner.Min;
            Max = _inner.Max;
            Median = _inner.Median;
            Average = _inner.Average;

            _medians.Add(_inner.Median.Value);
            _averages.Add(_inner.Average.Value);
            ClearCommand.NotifyCanExecuteChanged();
        });
    }

    private static LineSeries<double> CreateMedianSeries(ObservableCollection<double> values) => new() 
    {
        Values = values,
        Name = "Medians",
        Stroke = new SolidColorPaint(SkiaSharp.SKColors.Blue, 2),
        Fill = null,
        GeometryFill = null,
        GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.Blue, 2),
        GeometrySize = 1,
        LineSmoothness = 0,
        ZIndex = 1,
    };

    private static LineSeries<double> CreateAverageSeries(ObservableCollection<double> values) => new()
    {
        Values = values,
        Name = "Averages",
        Stroke = new SolidColorPaint(SkiaSharp.SKColors.Green, 2),
        GeometryFill = null,
        GeometryStroke = new SolidColorPaint(SkiaSharp.SKColors.Green, 2),
        Fill = null,
        GeometrySize = 1,
        LineSmoothness = 0,
        ZIndex = 0,
    };
}
