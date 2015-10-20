using System.Collections.Generic;

namespace Playverse.Data
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Batch<T>(
            this IEnumerable<T> source, int batchSize)
        {
            List<T> buffer = new List<T>(batchSize);

            foreach (T item in source)
            {
                buffer.Add(item);

                if (buffer.Count >= batchSize)
                {
                    yield return buffer;
                    buffer = new List<T>(batchSize);
                }
            }
            if (buffer.Count > 0)
            {
                yield return buffer;
            }
        }

    }
}