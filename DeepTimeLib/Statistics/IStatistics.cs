namespace DeepTime.Advisor.Statistics
{
    public interface IStatistics
    {
        int Count { get; }

        double? Max { get; }
        double? Median { get; }
        double? Average { get; }        
        double? Min { get; }
        IReadOnlyList<StatisticsEntry> Stats { get; }
        public void Submit(StatisticsEntry entry);

        void Clear();
    }
}