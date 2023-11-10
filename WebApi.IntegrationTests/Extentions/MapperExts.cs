using AutoMapper;

namespace WebApi.IntegrationTests.Extentions
{
    public static class MapperExts
    {
        public static IEnumerable<TDest> MapEntities<TSource, TDest>(this ResolutionContext resolutionContext,
            IEnumerable<TSource> source, IEnumerable<TDest> dest, Func<TSource, Guid> sourceKey, Func<TDest, Guid> destKey)
            where TSource : class
            where TDest : class
        {
            var mapper = resolutionContext.Mapper;

            if (source == null || !source.Any())
            {
                return new List<TDest>(0);
            }

            if (dest == null || !dest.Any())
            {
                return source.Select(x => mapper.Map<TDest>(x)).ToList();
            }

            var result = new List<TDest>(source.Count());

            foreach (var sourceItem in source)
            {
                var destItem = dest.FirstOrDefault(x => destKey(x) == sourceKey(sourceItem));

                if (destItem == null)
                {
                    result.Add(mapper.Map<TDest>(sourceItem));
                }
                else
                {
                    result.Add(mapper.Map(sourceItem, destItem));
                }
            }

            return result;
        }
    }
}
