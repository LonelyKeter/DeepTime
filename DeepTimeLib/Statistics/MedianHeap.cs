namespace DeepTime.Advisor.Statistics;

internal struct MedianHeap<T>
{
    private IComparer<T> _comparer;

    private readonly PriorityQueue<T, T> _minHeap;
    private readonly PriorityQueue<T, T> _maxHeap;

    public MedianHeap() : this(Comparer<T>.Default) { }

    public MedianHeap(IComparer<T> comparer)
    {
        _comparer = comparer;

        _minHeap = new(_comparer);
        _maxHeap = new(_comparer.Reverse());
    }

    public MedianHeap(IReadOnlyCollection<T> values, IComparer<T> comparer)
    {
        _comparer = comparer;

        var sorted = values.ToArray();
        Array.Sort(sorted, _comparer);

        _minHeap = new(sorted.Length / 2 + 1, _comparer);
        _maxHeap = new(sorted.Length / 2 + 1, _comparer.Reverse());

        var index = sorted.Length / 2;

        _minHeap.EnqueueRange(sorted[..index].Select(val => (val, val)));
        _maxHeap.EnqueueRange(sorted[index..].Select(val => (val, val)));
    }

    public MedianHeap(IReadOnlyCollection<T> values) 
        : this(values, Comparer<T>.Default) { }

    public T? PeekMedian()
    {
        if (_minHeap.Count == 0 && _maxHeap.Count == 0)
            return default;

        return _minHeap.Count > _maxHeap.Count ?
            _minHeap.Peek() :
            _maxHeap.Peek();
    }

    public void Clear()
    {
        _minHeap.Clear();
        _maxHeap.Clear();
    }


    public void Push(T value)
    {
        if (_comparer.Compare(value, PeekMedian()) > 0)
        {
            _minHeap.Enqueue(value, value);
        }
        else
        {
            _maxHeap.Enqueue(value, value);
        }

        Rebalance();
    }

    private void Rebalance()
    {
        if (Math.Abs(_minHeap.Count - _maxHeap.Count) <= 1)
            return;

        if (_minHeap.Count > _maxHeap.Count)
        {
            Rebalance(_minHeap, _maxHeap);
        }
        else
        {
            Rebalance(_maxHeap, _minHeap);
        }
    }

    private static void Rebalance(PriorityQueue<T, T> greater, PriorityQueue<T, T> smaller)
    {
        var val = greater.Dequeue();
        smaller.Enqueue(val, val);
    }    
}
