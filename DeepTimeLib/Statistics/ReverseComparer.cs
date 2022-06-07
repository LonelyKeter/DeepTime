namespace DeepTime.Advisor.Statistics;

internal class ReverseComparer<T> : IComparer<T>
{
    private readonly IComparer<T> _inner;
    
    public ReverseComparer(IComparer<T> comparer)
    {
        _inner = comparer;
    }

    public int Compare(T? x, T? y) => -_inner.Compare(x, y);    
}

internal static class ReverseComparerExtensions
{
    public static ReverseComparer<T> Reverse<T>(this IComparer<T> comparer)
        => new(comparer);
}