using System.Collections;

namespace Synercoding.FileFormats.Pdf.Internals;

internal sealed class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    where T1 : notnull
    where T2 : notnull
{
    private readonly IDictionary<T1, T2> _forward = new Dictionary<T1, T2>();
    private readonly IDictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

    public Map()
    {
        Forward = new Indexer<T1, T2>(_forward);
        Reverse = new Indexer<T2, T1>(_reverse);
    }

    public int Count => _forward.Count;

    public void Add(T1 t1, T2 t2)
    {
        _forward.Add(t1, t2);
        _reverse.Add(t2, t1);
    }

    public void Clear()
    {
        _forward.Clear();
        _reverse.Clear();
    }

    public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
    {
        return _forward.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Indexer<T1, T2> Forward { get; }
    public Indexer<T2, T1> Reverse { get; }

    public sealed class Indexer<T3, T4> : IReadOnlyDictionary<T3, T4>
        where T3 : notnull
        where T4 : notnull
    {
        private readonly IDictionary<T3, T4> _dictionary;

        public Indexer(IDictionary<T3, T4> dictionary)
        {
            _dictionary = dictionary;
        }

        public T4 this[T3 index]
        {
            get => _dictionary[index];
            set => _dictionary[index] = value;
        }

        public IEnumerable<T3> Keys => _dictionary.Keys;

        public IEnumerable<T4> Values => _dictionary.Values;

        public int Count => _dictionary.Count;

        public bool Contains(T3 value)
            => _dictionary.ContainsKey(value);

        public bool ContainsKey(T3 key)
            => _dictionary.ContainsKey(key);

        public IEnumerator<KeyValuePair<T3, T4>> GetEnumerator()
            => _dictionary.GetEnumerator();

        public bool TryGetValue(T3 key, out T4 value)
            => throw new InvalidOperationException("TryGetValue is not supported on types with non-nullable values.");

        IEnumerator IEnumerable.GetEnumerator()
            => _dictionary.GetEnumerator();
    }
}
