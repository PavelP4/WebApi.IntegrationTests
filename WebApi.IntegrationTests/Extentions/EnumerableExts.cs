namespace WebApi.IntegrationTests.Extentions
{
    public static class EnumerableExts
    {
        public static bool EqualsTo<T, K>(this IEnumerable<T> left, IEnumerable<T> right, Func<T, K> byKey)
        {
            if (left == right)
            {
                return true;
            }

            return left != null && right != null
                && left.Count() == right.Count()
                && (left.Count() == 0 || left.OrderBy(byKey).SequenceEqual(right.OrderBy(byKey)));
        }

        public static int GetCombinedHashCode<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                return 0;
            }

            return sequence.Aggregate(new HashCode(), (hc, x) => {
                hc.Add(x);
                return hc;
            }).ToHashCode();
        }
    }
}
