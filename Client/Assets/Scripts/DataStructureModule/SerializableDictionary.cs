namespace System.Collections.Generic
{
    public class SerializableDictionary<TKey, TValue>: IEnumerable<KeyValuePair<TKey, TValue>>
    {
        public Dictionary<TKey, TValue> Container = new();

        public TValue this[TKey key]
        {
            get => Container[key];
            set => Container[key] = value;
        }
        
        public void Add(TKey key, TValue value)
        {
            Container.Add(key, value);
        }

        public bool ContainKey(TKey key)
        {
            return Container.ContainsKey(key);
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            return Container.TryGetValue(key, out value);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Container.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}