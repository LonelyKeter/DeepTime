using System.Collections.Generic;
using System.Linq;

namespace DeepTime.Simulator.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using DeepTime.Advisor.Data;
using DeepTime.Advisor.Statistics;
using DeepTime.Simulator.Messages;
using DeepTime.Simulator.Services;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;

using SkiaSharp;

using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

public sealed partial class StatisticsVM : ObservableValidator,
    IRecipient<DayFinishedMessage>
{
    private readonly Statistics _inner = new();

    public ObservableStatisticData Data { get; } = new();
    public StatisticsPlotConfig PlotConfig { get; }

    public IMessenger Messenger { get; }

    #region Observable props 
    [ObservableProperty]
    double? _medianReward;
    [ObservableProperty]
    double? _averageReward;

    [ObservableProperty]
    double? _veryHighPriorityAverageComplition;
    [ObservableProperty]
    double? _highPriorityAverageComplition;
    [ObservableProperty]
    double? _mediumPriorityAverageComplition;
    [ObservableProperty]
    double? _lowPriorityAverageComplition;
    [ObservableProperty]
    double? _veryLowPriorityAverageComplition;
    [ObservableProperty]
    double? _totalAverageComplition;

    bool _rewardsVisible = true;
    public bool RewardsVisible
    {
        get => _rewardsVisible;
        set
        {
            if (SetProperty(ref _rewardsVisible, value))
            {
                PlotConfig.SetRewardVisibility(value);
            }
        }
    }

    bool _taskPercentageVisible = true;
    public bool TaskPercentageVisible
    {
        get => _taskPercentageVisible;
        set
        {
            if (SetProperty(ref _taskPercentageVisible, value))
            {
                PlotConfig.SetTaskPercentageVisibility(value);
            }
        }
    }

    [Required]
    [Range(0, int.MaxValue)]
    public int AccountFor
    {
        get => _inner.AccountFor;
        set
        {
            if (SetProperty(_inner.AccountFor, value, _inner, (i, a) => i.AccountFor = a, true))
            {
                UpdateObservables(_inner.StatisticValues);
            }
        }
    }
    #endregion

    public StatisticsVM(IMessenger messenger)
    {
        Messenger = messenger;

        PlotConfig = new(
            StatisticsPlotConfig.RewardSeries(Data.RewardPlotParams()),
            StatisticsPlotConfig.TaskPercentageSeries(Data.TaskPercentagePlotParams())
            );

        Messenger.RegisterAll(this);
    }

    [ICommand(CanExecute = nameof(CanClear))]
    public void Clear()
    {
        Data.Clear();

        MedianReward = null;
        AverageReward = null;

        VeryHighPriorityAverageComplition = null;
        HighPriorityAverageComplition = null;
        MediumPriorityAverageComplition = null;
        LowPriorityAverageComplition = null;
        VeryLowPriorityAverageComplition = null;
        TotalAverageComplition = null;

        ClearCommand.NotifyCanExecuteChanged();
    }
    bool CanClear => _inner.Count > 0;

    public void Receive(DayFinishedMessage message)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            var entry = message.Entry;
            _inner.Submit(entry);

            Data.Submit(entry, _inner.StatisticValues.Value);

            UpdateObservables(_inner.StatisticValues.Value);

            if (_inner.Count == 1) ClearCommand.NotifyCanExecuteChanged();
        });
    }

    private void UpdateObservables(StatisticValues? values)
    {
        if (!values.HasValue) return;

        MedianReward = values.Value.MedianReward;
        AverageReward = values.Value.AverageReward;

        VeryHighPriorityAverageComplition = values.Value.AverageTaskComplition[Priority.VeryHigh.AsIndex()];
        HighPriorityAverageComplition = values.Value.AverageTaskComplition[Priority.High.AsIndex()];
        MediumPriorityAverageComplition = values.Value.AverageTaskComplition[Priority.Medium.AsIndex()];
        LowPriorityAverageComplition = values.Value.AverageTaskComplition[Priority.Low.AsIndex()];
        VeryLowPriorityAverageComplition = values.Value.AverageTaskComplition[Priority.VeryLow.AsIndex()];
        TotalAverageComplition = values.Value.TotalTaskComplition;
    }
}

[INotifyPropertyChanged]
public sealed partial class ObservableStatisticData
{
    [ObservableProperty]
    ObservableCollection<StatisticsEntry> _entries = new();

    [ObservableProperty]
    ObservableCollection<double> _rewardMedian = new();
    [ObservableProperty]
    ObservableCollection<double> _rewardAverage = new();

    [ObservableProperty]
    ObservableCollection<double>[] _taskPercentageAverages = new ObservableCollection<double>[Types.PriorityCount];
    [ObservableProperty]
    ObservableCollection<double> _totalTaskPercentageAverages = new();

    public int Count => _entries.Count;

    private int _maxCount = 1000;
    public int MaxCount
    {
        get => _maxCount;
        set
        {
            if (_maxCount != value)
            {
                _maxCount = value;
                TrimExcess();
                OnPropertyChanged(nameof(MaxCount));
            }
        }
    }
    public ObservableStatisticData()
    {
        foreach (var priority in Types.PriorityValues)
        {
            TaskPercentageAverages[priority.AsIndex()] = new();
        }
    }

    public void Clear()
    {
        Entries.Clear();
        RewardMedian.Clear();
        RewardAverage.Clear();
        TotalTaskPercentageAverages.Clear();
        foreach (var collection in TaskPercentageAverages)
        {
            collection.Clear();
        }
    }

    public void Submit(StatisticsEntry entry, StatisticValues values)
    {
        Entries.Add(entry);
        RewardMedian.Add(values.MedianReward);
        RewardAverage.Add(values.AverageReward);
        TotalTaskPercentageAverages.Add(values.TotalTaskComplition);

        foreach (var pr in Types.PriorityValues)
        {
            TaskPercentageAverages[pr.AsIndex()].Add(values.AverageTaskComplition[pr.AsIndex()]);
        }

        TrimExcess();
    }

    private void TrimExcessInPlace<T>(ObservableCollection<T> old, int excessCount)
    {
        for (var i = 0; i < excessCount; i++)
        {
            old.RemoveAt(0);
        }
    }
    private ObservableCollection<T> TrimExcess<T>(ObservableCollection<T> old, int excessCount)
        => new(old.Skip(excessCount));

    private void TrimExcess(int excessCount)
    {
        Entries = TrimExcess(Entries, excessCount);
        RewardMedian = TrimExcess(RewardMedian, excessCount);
        RewardAverage = TrimExcess(RewardAverage, excessCount);
        TotalTaskPercentageAverages = TrimExcess(TotalTaskPercentageAverages, excessCount);

        var newTaskPercentAverages = new ObservableCollection<double>[Types.PriorityCount];

        foreach (var index in Types.PriorityValues.Select(p => p.AsIndex()))
        {
            newTaskPercentAverages[index] = TrimExcess(TaskPercentageAverages[index], excessCount);
        }

        TaskPercentageAverages = newTaskPercentAverages;
    }
    private void TrimExcessInPLace(int excessCount)
    {
        TrimExcessInPlace(Entries, excessCount);
        TrimExcessInPlace(RewardMedian, excessCount);
        TrimExcessInPlace(RewardAverage, excessCount);
        TrimExcessInPlace(TotalTaskPercentageAverages, excessCount);

        foreach (var index in Types.PriorityValues.Select(p => p.AsIndex()))
        {
            TrimExcessInPlace(TaskPercentageAverages[index], excessCount);
        }
    }

    private void TrimExcess()
    {
        var excessCount = Count - MaxCount;
        if (excessCount <= 0) return;

        if (excessCount > System.Math.Max(Count / 2, 30))
        {
            TrimExcess(excessCount);
        }
        else
        {
            TrimExcessInPLace(excessCount);
        }
    }


    public IEnumerable<(ObservableCollection<double> values, SKColor color, string name)> RewardPlotParams() => new[]
    {
        (RewardMedian, SKColors.Blue, "Reward median"),
        (RewardAverage, SKColors.Purple, "Reward average"),
    };

    public IEnumerable<(ObservableCollection<double> values, SKColor color, string name)> TaskPercentagePlotParams()
    {
        foreach (var priority in Types.PriorityValues)
        {
            yield return (
                TaskPercentageAverages[priority.AsIndex()],
                PriorityColor(priority),
                $"{priority} pirority task complition %");
        }

        yield return (TotalTaskPercentageAverages, SKColors.Black, "Total task complition %");
    }

    private static SKColor PriorityColor(Priority priority)
    {
        var interpolation = (double)priority / (double)Priority.VeryHigh;

        var red = (byte)(interpolation * SKColors.Red.Red + (1.0 - interpolation) * SKColors.Green.Red);
        var green = (byte)(interpolation * SKColors.Red.Green + (1.0 - interpolation) * SKColors.Green.Green);
        var blue = (byte)(interpolation * SKColors.Red.Blue + (1.0 - interpolation) * SKColors.Green.Blue);

        return new(red, green, blue);
    }
}

public sealed class StatisticsPlotConfig
{
    private static readonly Paint TextPaint = new SolidColorPaint(SKColors.Black);
    private static readonly double NameTextSize = 15;
    private static readonly double TextSize = 10;


    private readonly Axis _rewardAxis = new()
    {
        Name = "Reward value",
        MinStep = 0.1,
        NameTextSize = NameTextSize,
        TextSize = TextSize,
        NamePaint = TextPaint,
        LabelsPaint = TextPaint,
    };
    private readonly Axis _taskPercentageAxis = new()
    {
        Name = "% of tasks Done",
        MinStep = 1.0,
        NameTextSize = NameTextSize,
        TextSize = TextSize,
        NamePaint = TextPaint,
        LabelsPaint = TextPaint,
    };

    private readonly ISeries[] _rewardSeries;
    private readonly ISeries[] _taskPercentageSeries;

    public ISeries[] Series { get; }


    public Axis[] XAxes { get; }
    public Axis[] YAxes { get; }

    public StatisticsPlotConfig(ISeries[] rewardSeries, ISeries[] taskPercentageSeries)
    {
        _rewardSeries = rewardSeries;
        _taskPercentageSeries = taskPercentageSeries;

        Series = rewardSeries.Concat(taskPercentageSeries).ToArray();

        YAxes = new[]
        {
            _rewardAxis,
            _taskPercentageAxis,
        };
        XAxes = new[]
        {
            new Axis
            {
                Name = "Episodes",
                MinStep = 1,
                NameTextSize = NameTextSize,
                TextSize = TextSize,
                NamePaint = TextPaint,
                LabelsPaint = TextPaint,
            }
        };
    }

    public void SetRewardVisibility(bool visibility)
    {
        _rewardAxis.IsVisible = visibility;

        foreach (var series in _rewardSeries)
        {
            series.IsVisible = visibility;
        }
    }

    public void SetTaskPercentageVisibility(bool visibility)
    {
        _taskPercentageAxis.IsVisible = visibility;

        foreach (var series in _taskPercentageSeries)
        {
            series.IsVisible = visibility;
        }
    }

    public static LineSeries<double>[] RewardSeries(IEnumerable<(ObservableCollection<double> values, SKColor color, string name)> tuples)
        => tuples.Select(tuple => RewardSeries(tuple.values, tuple.color, tuple.name)).ToArray();
    public static LineSeries<double> RewardSeries(ObservableCollection<double> values, SKColor color, string name) =>
        LineSeries(values, color, name, 0);

    public static LineSeries<double>[] TaskPercentageSeries(IEnumerable<(ObservableCollection<double> values, SKColor color, string name)> tuples)
     => tuples.Select(tuple => TaskPercentageSeries(tuple.values, tuple.color, tuple.name)).ToArray();
    public static LineSeries<double> TaskPercentageSeries(ObservableCollection<double> values, SKColor color, string name) =>
        LineSeries(values, color, name, 1);

    public static LineSeries<double> LineSeries(ObservableCollection<double> values, SKColor color, string name, int axis) => new()
    {
        Values = values,
        Name = name,
        Stroke = new SolidColorPaint(color, 1),
        Fill = null,
        GeometryFill = null,
        GeometryStroke = new SolidColorPaint(color, 1),
        GeometrySize = 1,
        LineSmoothness = 0,
        ZIndex = 1,
        ScalesYAt = axis
    };
}