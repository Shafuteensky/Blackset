using System;
using System.Collections.Generic;

namespace Extensions.Helpers
{
    /// <summary>
    /// Хелпер для копирования коллекций
    /// </summary>
    public static class CollectionCopy
    {
        /// <summary>
        /// Shallow-копия списка (копируются ссылки/значения элементов)
        /// </summary>
        public static List<T> ListShallow<T>(IList<T> source)
        {
            if (source == null)
            {
                return null;
            }

            return new List<T>(source);
        }

        /// <summary>
        /// Deep-копия списка через фабрику копирования элемента
        /// </summary>
        public static List<T> ListDeep<T>(IList<T> source, Func<T, T> cloneItem)
        {
            if (source == null)
            {
                return null;
            }

            if (cloneItem == null)
            {
                throw new ArgumentNullException(nameof(cloneItem));
            }

            List<T> copy = new List<T>(source.Count);

            for (int i = 0; i < source.Count; i++)
            {
                copy.Add(cloneItem(source[i]));
            }

            return copy;
        }

        /// <summary>
        /// Shallow-копия словаря (копируются ссылки/значения ключей и значений)
        /// </summary>
        public static Dictionary<TKey, TValue> DictionaryShallow<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            if (source == null)
            {
                return null;
            }

            return new Dictionary<TKey, TValue>(source);
        }

        /// <summary>
        /// Deep-копия словаря через фабрики копирования ключа и значения
        /// </summary>
        public static Dictionary<TKey, TValue> DictionaryDeep<TKey, TValue>(
            IDictionary<TKey, TValue> source,
            Func<TKey, TKey> cloneKey,
            Func<TValue, TValue> cloneValue)
        {
            if (source == null)
            {
                return null;
            }

            if (cloneKey == null)
            {
                throw new ArgumentNullException(nameof(cloneKey));
            }

            if (cloneValue == null)
            {
                throw new ArgumentNullException(nameof(cloneValue));
            }

            Dictionary<TKey, TValue> copy = new Dictionary<TKey, TValue>(source.Count);

            foreach (var kvp in source)
            {
                copy.Add(cloneKey(kvp.Key), cloneValue(kvp.Value));
            }

            return copy;
        }

        /// <summary>
        /// Копия словаря, где значения — списки (частый кейс)
        /// </summary>
        public static Dictionary<TKey, List<TItem>> DictionaryOfLists<TKey, TItem>(
            IDictionary<TKey, List<TItem>> source)
        {
            if (source == null)
            {
                return null;
            }

            Dictionary<TKey, List<TItem>> copy = new Dictionary<TKey, List<TItem>>(source.Count);

            foreach (var kvp in source)
            {
                copy.Add(kvp.Key, kvp.Value == null ? null : new List<TItem>(kvp.Value));
            }

            return copy;
        }
    }
}
