namespace DeepTime.Advisor.Statistics
{
    public interface IStatistics
    {
        int Count { get; }

        double? MedianReward { get; }
        double? AverageReward { get; }  
        public void Submit(StatisticsEntry entry);

        void Clear();
    }
}