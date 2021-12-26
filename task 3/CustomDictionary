using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp29
{

    public class CustomDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private static int DEFAULT_CAPACITY = 15;
        private static KeyValuePair<TKey, TValue> NULL_KEY_VALUE_PAIR = new KeyValuePair<TKey, TValue>();
        private List<KeyValuePair<TKey, TValue>>[] backets;
        public int Count { get; set; } = 0;
        public bool IsReadOnly { get; } = false;

        public CustomDictionary()
        {
            ensureCapasity(DEFAULT_CAPACITY);
        }

        public CustomDictionary(bool IsReadOnly) : this()
        {
            this.IsReadOnly = IsReadOnly;
        }

        private void ensureCapasity(int newBacketsCount)
        {
            List<KeyValuePair<TKey, TValue>>[] newBackets = new List<KeyValuePair<TKey, TValue>>[newBacketsCount];

            if (backets != null)
            {
                foreach (var backet in backets)
                {
                    if (backet != null)
                    {
                        foreach (var elem in backet)
                        {
                            storeInBacket(newBackets, elem);
                        }
                    }
                }
            }

            backets = newBackets;
        }

        private void storeInBacket(KeyValuePair<TKey, TValue> elem)
        {
            storeInBacket(backets, elem);
        }

        private bool storeInBacket(List<KeyValuePair<TKey, TValue>>[] backets, KeyValuePair<TKey, TValue> elem)
        {
            List<KeyValuePair<TKey, TValue>> backet = tryGetFromBacket(backets, getBacketIndex(elem));

            if (!backet.Contains(elem))
            {
                backet.Add(elem);
                return true;
            }

            return false;
        }

        private int getBacketIndex(KeyValuePair<TKey, TValue> item)
        {
            return getBacketIndex(item.Key);
        }

        private int getBacketIndex(TKey key)
        {
            return Math.Abs(key.GetHashCode() % backets.Count());
        }

        private List<KeyValuePair<TKey, TValue>> tryGetFromBacket(int backetIndex)
        {
            return tryGetFromBacket(backets, backetIndex);
        }


        private List<KeyValuePair<TKey, TValue>> tryGetFromBacket(List<KeyValuePair<TKey, TValue>>[] backets, int backetIndex)
        {
            if (backetIndex < 0 || backetIndex > backets.Count() - 1)
                throw new ArgumentOutOfRangeException();

            List<KeyValuePair<TKey, TValue>> backet = backets[backetIndex];

            if (backet == null)
            {
                backet = new List<KeyValuePair<TKey, TValue>>();
                backets[backetIndex] = backet;
            }

            return backet;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return new CustomDictionaryEnumerator<TKey, TValue>(backets);
        }

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (IsReadOnly) throw new AccessViolationException();
            storeInBacket(backets, item);
            Count++;
        }

        public void Clear()
        {
            if (IsReadOnly) throw new AccessViolationException();
            foreach (var backet in backets)
            {
                if (backet != null)
                    backet.Clear();
            }

            Count = 0;
        }


        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            int index = getBacketIndex(item.Key);

            foreach (var fromBacket in tryGetFromBacket(index))
            {
                if (fromBacket.Key.Equals(item.Key) &&
                    fromBacket.Value.Equals(item.Value))
                {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsKey(TKey key)
        {
            int index = getBacketIndex(key);

            foreach (var fromBacket in tryGetFromBacket(index))
            {
                if (fromBacket.Key.Equals(key)) return true;
            }

            return false;
        }


        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array.Length - arrayIndex < Count)
                throw new ArrayTypeMismatchException();

            int index = arrayIndex;

            foreach (var backet in backets)
            {
                if (backet != null)
                {
                    foreach (var item in backet)
                    {
                        array[index] = item;
                        index++;
                    }
                }
            }
        }

        public bool Remove(TKey key)
        {
            if (IsReadOnly) throw new AccessViolationException();

            int index = getBacketIndex(key);

            KeyValuePair<TKey, TValue> itemToDelete = NULL_KEY_VALUE_PAIR;

            List<KeyValuePair<TKey, TValue>> backet = tryGetFromBacket(index);

            foreach (var fromBacket in backet)
            {
                if (fromBacket.Key.Equals(key)) itemToDelete = fromBacket;
            }

            if (itemToDelete.Equals(NULL_KEY_VALUE_PAIR)) return false;

            backet.Remove(itemToDelete);
            Count--;
            return true;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            if (IsReadOnly) throw new AccessViolationException();

            int index = getBacketIndex(item.Key);

            KeyValuePair<TKey, TValue> itemToDelete = NULL_KEY_VALUE_PAIR;

            List<KeyValuePair<TKey, TValue>> backet = tryGetFromBacket(index);

            foreach (var fromBacket in backet)
            {
                if (fromBacket.Key.Equals(item.Key) &&
                    fromBacket.Value.Equals(item.Value)) itemToDelete = fromBacket;
            }

            if (itemToDelete.Equals(NULL_KEY_VALUE_PAIR)) return false;

            backet.Remove(itemToDelete);
            Count--;
            return true;
        }

        //методы IDictionary 
        public bool TryGetValue(TKey key, out TValue value)
        {
            foreach (var item in tryGetFromBacket(getBacketIndex(key)))
            {
                if (item.Key.Equals(key))
                {
                    value = item.Value;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private TValue get(TKey key)
        {
            TryGetValue(key, out TValue result);
            return result;
        }

        public TValue this[TKey key]
        {
            get => get(key);
            set => Add(key, value);
        }

        public ICollection<TKey> KeySet()
        {
            ISet<TKey> keys = new HashSet<TKey>();
            foreach (var backet in backets)
            {
                if (backet != null)
                {
                    foreach (var item in backet)
                    {
                        keys.Add(item.Key);
                    }
                }
            }

            return keys;
        }


        public ICollection<TValue> ValueSet()
        {
            ISet<TValue> values = new HashSet<TValue>();
            foreach (var backet in backets)
            {
                if (backet != null)
                {
                    foreach (var item in backet)
                    {
                        values.Add(item.Value);
                    }
                }
            }

            return values;
        }

        public ICollection<TKey> Keys => KeySet();
        public ICollection<TValue> Values => ValueSet();
    }

    class CustomDictionaryEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        private static IEnumerator<KeyValuePair<TKey, TValue>> DUMMY_ENUMERATOR = new DummyEnumerator<TKey, TValue>();
        private IEnumerator<KeyValuePair<TKey, TValue>>[] enumerators;
        private int index;

        public CustomDictionaryEnumerator(List<KeyValuePair<TKey, TValue>>[] backets)
        {
            enumerators = new IEnumerator<KeyValuePair<TKey, TValue>>[backets.Length];
            for (int i = 0; i < backets.Length; i++)
            {
                enumerators[i] = backets[i] != null ? backets[i].GetEnumerator() : DUMMY_ENUMERATOR;
            }

            index = 0;
        }

        public bool MoveNext()
        {
            for (int i = index; i < enumerators.Length; i++)
            {
                if (enumerators[i].MoveNext())
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            index = 0;
            foreach (var enumerator in enumerators)
            {
                enumerator.Reset();
            }
        }

        public KeyValuePair<TKey, TValue> Current => enumerators[index].Current;

        object IEnumerator.Current => enumerators[index].Current;

        public void Dispose()
        {
            foreach (var enumerator in enumerators)
            {
                enumerator.Dispose();
            }
        }


    }

    class DummyEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
    {
        public bool MoveNext()
        {
            return false;
        }

        public void Reset()
        {
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }

        public KeyValuePair<TKey, TValue> Current { get; }
    }
}
