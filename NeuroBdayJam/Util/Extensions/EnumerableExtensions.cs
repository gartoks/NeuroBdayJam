using NeuroBdayJam.Util.Math.Random;
using System.Collections.ObjectModel;

namespace NeuroBdayJam.Util.Extensions;
public static class EnumerableExtensions {
    public static ObservableCollection<T> AsObservableCollection<T>(this IEnumerable<T> items) => new ObservableCollection<T>(items);

    public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items) {
        items.ForEach(collection.Add);
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> en, Action<T> action) {
        foreach (T? item in en) {
            action(item);
        }
        return en;
    }

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> en, Action<T, int> action) {
        int i = 0;
        foreach (T? item in en) {
            action(item, i);
            i++;
        }
        return en;
    }

    public static int IndexOf<T>(this IReadOnlyList<T> list, T item) {
        int i = 0;
        foreach (T element in list) {
            if (Equals(element, item))
                return i;
            i++;
        }
        return -1;
    }

    public static IList<T> Move<T>(this IList<T> list, T item, bool up) {
        int jobIndex = list.IndexOf(item);
        if (up && jobIndex > 0)
            list.Swap(jobIndex, jobIndex - 1);
        else if (!up && jobIndex < list.Count - 1)
            list.Swap(jobIndex, jobIndex + 1);
        return list;
    }

    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB) {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }

    public static T As<T>(this object[] array, int index) => (T)array[index];

    public static T[] Extend<T>(this T[] a, T[] b) {
        T[] array = new T[a.Length + b.Length];
        Array.Copy(a, 0, array, 0, a.Length);
        Array.Copy(b, 0, array, a.Length, b.Length);
        return array;
    }

    public static T[] SubArray<T>(this T[] data, int index, int length) {
        T[] result = new T[length];
        Array.Copy(data, index, result, 0, length);
        return result;
    }

    public static void ForEach<T>(this T[,] array, Action<int, int, T> action) {
        for (int i = 0; i < array.GetLength(0); i++) {
            for (int j = 0; j < array.GetLength(1); j++) {
                action(i, j, array[i, j]);
            }
        }
    }

    public static void AddNested<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> d, K k, K2 k2, V v) where K2 : notnull {
        if (!d.TryGetValue(k, out Dictionary<K2, V>? d2)) {
            d2 = new Dictionary<K2, V>();
            d[k] = d2;
        }

        d2[k2] = v;
    }

    public static bool TryGetNested<K, K2, V>(this Dictionary<K, Dictionary<K2, V>> d, K k, K2 k2, out V v) where K2 : notnull {
        v = default!;

        return d.TryGetValue(k, out Dictionary<K2, V>? d2) && d2.TryGetValue(k2, out v);
    }

    public static T[] Flatten<T>(this T[,] array) {
        T[] res = new T[array.GetLength(0) * array.GetLength(1)];
        for (int yi = 0; yi < array.GetLength(1); yi++) {
            for (int xi = 0; xi < array.GetLength(0); xi++) {
                res[xi + yi * array.GetLength(0)] = array[xi, yi];
            }
        }
        return res;
    }

    public static T[,] Expand<T>(this T[] array, int width, int height) {
        T[,] res = new T[width, height];

        for (int i = 0; i < array.Length; i++) {
            int xi = i % width;
            int yi = i / width;

            res[xi, yi] = array[i];
        }

        return res;
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> en, Random random) {
        return en.OrderBy(e => random.Next());
    }

    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> en, IRandomGenerator random) {
        return en.OrderBy(e => random.Next());
    }
}