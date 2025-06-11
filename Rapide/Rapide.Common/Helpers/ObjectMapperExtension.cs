using AutoMapper;

namespace Rapide.Common.Helpers
{
    public static class ObjectMapperExtension
    {
        private static IMapper CreateMapper(Type source, Type dest, IMapper? mapper = null)
        {
            return mapper ?? new MapperConfiguration(cfg => cfg.CreateMap(source, dest))
                .CreateMapper();
        }

        /// <summary>
        /// Map the properties values from source into a new instance of TDest
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDest Map<TDest>(this object source, IMapper? mapper = null) where TDest : class
        {
            var map = CreateMapper(source.GetType(), typeof(TDest), mapper);

            return map.Map<TDest>(source);
        }

        /// <summary>
        /// Update properties value with the same name, with the values from the source object
        /// </summary>
        /// <typeparam name="TDest"></typeparam>
        /// <param name="entity">The target object</param>
        /// <param name="source">The value source</param>
        /// <param name="mapper">Custom Object Mapper Configuration</param>
        /// <returns>Instance of the target object</returns>
        public static TDest UpdateFields<TDest>(this TDest entity, object? source, IMapper? mapper = null)
        {
            if (source == null)
                return entity;
            var map = CreateMapper(source.GetType(), entity.GetType(), mapper);

            return map.Map(source, entity);
        }
    }
}