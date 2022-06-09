using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepTime.Advisor.Statistics;


internal struct MedianList<T>
{
    private readonly IComparer<T> _comparer;
    private readonly List<T> _inner;

    public int Count => _inner.Count;
    public T? Median => Count > 0 ? _inner[Count / 2] : default;

    public MedianList() : this(Comparer<T>.Default) { }

    public MedianList(IComparer<T> comparer)
    {
        _comparer = comparer;
        _inner = new();
    }

    public MedianList(IReadOnlyCollection<T> values, IComparer<T> comparer)
    {
        _comparer = comparer;
        _inner = values.ToList();
        _inner.Sort(_comparer);
    }

    public MedianList(IReadOnlyCollection<T> values)
        : this(values, Comparer<T>.Default) { }

    public void Clear()
    {
        _inner.Clear();
    }

    public void Add(T value)
    {
        var insertIndex = _inner.BinarySearch(value, _comparer);

        if (insertIndex < 0) insertIndex = ~insertIndex;
        _inner.Insert(insertIndex, value);
    }

    public bool Remove(T value) 
    {
        return _inner.Remove(value);
    }
}
