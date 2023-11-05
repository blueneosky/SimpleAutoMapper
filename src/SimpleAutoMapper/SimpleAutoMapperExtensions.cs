using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleAutoMapper
{
    public static class SimpleAutoMapperExtensions
    {
        /// <summary>
        /// Get the mapping definition.
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <returns></returns>
        public static ITypeMapping<TSrc, TDst> GetTypeMapping<TSrc, TDst>(this ISimpleAutoMapper mapper)
               where TSrc : class
               where TDst : class
               => (ITypeMapping<TSrc, TDst>)mapper.GetTypeMapping(typeof(TSrc), typeof(TDst));

        /// <summary>
        /// Get the mapper delegate
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <returns></returns>
        public static Func<TSrc?, TDst?> GetMapper<TSrc, TDst>(this ISimpleAutoMapper mapper)
            where TSrc : class
            where TDst : class
            => (Func<TSrc?, TDst?>)mapper.GetMapper(typeof(TSrc), typeof(TDst));

        /// <summary>
        /// Map value <paramref name="src"/> to <typeparamref name="TDst"/>
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDst? Map<TSrc, TDst>(this ISimpleAutoMapper mapper, TSrc? src)
            where TSrc : class
            where TDst : class
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return mapper.GetMapper<TSrc, TDst>().Invoke(src);
        }

        /// <summary>
        /// Map the collection <paramref name="srcCollection"/> to a collection of <typeparamref name="TDst"/>
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="srcCollection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ICollection<TDst?>? MapCollection<TSrc, TDst>(this ISimpleAutoMapper mapper, IEnumerable<TSrc?>? srcCollection)
              where TSrc : class
              where TDst : class
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            var selector = mapper.GetMapper<TSrc, TDst>();
            return srcCollection?.Select(selector).ToList();
        }

        /// <summary>
        /// Map value <paramref name="src"/> to <typeparamref name="TDst"/>.
        /// This is a runtime type solving (type of the <paramref name="src"/> instance)
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static TDst? MapTo<TDst>(this ISimpleAutoMapper mapper, object src)
            where TDst : class
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _ = src ?? throw new ArgumentNullException(nameof(src));
            var srcType = src.GetType();
            var dstType = typeof(TDst);
            var @delegate = mapper.GetMapper(srcType, dstType);
            return (TDst?)MappingDelegateRunner.Run(srcType, dstType, @delegate, src);
        }

        /// <summary>
        /// Map the collection <paramref name="srcCollection"/> to a collection of <typeparamref name="TDst"/>.
        /// This is a runtime type solving (generic type of the <paramref name="src"/> instance)
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="srcCollection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ICollection<TDst?>? MapCollectionTo<TDst>(this ISimpleAutoMapper mapper, IEnumerable<object> srcCollection)
              where TDst : class
        {
            _ = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _ = srcCollection ?? throw new ArgumentNullException(nameof(srcCollection));

            var srcType = srcCollection.GetType().GenericTypeArguments[0];
            var dstType = typeof(TDst);
            var @delegate = mapper.GetMapper(srcType, dstType);
            var func = MappingDelegateRunner.GetFunc(srcType, dstType, @delegate);
            return srcCollection.Select(func).Cast<TDst?>().ToList();
        }

        /// <summary>
        /// Used to map value in fluent syntax : <code>mapper.MapFrom(src).To&lt;TDst&gt;()</code>
        /// This is a static type solving (type declaration of <paramref name="src"/>).
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        public static IMappingSource<TSrc> MapFrom<TSrc>(this ISimpleAutoMapper mapper, TSrc? src)
            where TSrc : class
            => new MappingSource<TSrc>(mapper, src);

        /// <summary>
        /// Used to map collection of values in fluent syntax : <code>mapper.MapFromCollection(src).To&lt;TDst&gt;()</code>
        /// This is a static type solving (generic type declaration of <paramref name="srcCollection"/>).
        /// </summary>
        /// <typeparam name="TSrc"></typeparam>
        /// <param name="mapper"></param>
        /// <param name="srcCollection"></param>
        /// <returns></returns>
        public static IMappingCollectionSource<TSrc> MapFromCollection<TSrc>(this ISimpleAutoMapper mapper, IEnumerable<TSrc> srcCollection)
            where TSrc : class
            => new MappingCollectionSource<TSrc>(mapper, srcCollection);

        private sealed class MappingSource<TSrc> : IMappingSource<TSrc>
            where TSrc : class
        {
            private ISimpleAutoMapper _mapper;
            private TSrc? _src;

            public MappingSource(ISimpleAutoMapper mapper, TSrc? src) => (_mapper, _src) = (mapper, src);

            public TDst? To<TDst>()
                where TDst : class
                => _mapper.Map<TSrc, TDst>(_src);
        }

        private sealed class MappingCollectionSource<TSrc> : IMappingCollectionSource<TSrc>
            where TSrc : class
        {
            private ISimpleAutoMapper _mapper;
            private IEnumerable<TSrc> _srcCollection;

            public MappingCollectionSource(ISimpleAutoMapper mapper, IEnumerable<TSrc> srcCollection) => (_mapper, _srcCollection) = (mapper, srcCollection);

            public ICollection<TDst?>? To<TDst>()
                where TDst : class
                => _mapper.MapCollection<TSrc, TDst>(_srcCollection);
        }
    }

    public interface IMappingSource<TSrc>
        where TSrc : class
    {
        /// <summary>
        /// Destination type for a transformation from <typeparamref name="TSrc"/> to <typeparamref name="TDst"/>
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <returns></returns>
        TDst? To<TDst>() where TDst : class;
    }

    public interface IMappingCollectionSource<TSrc>
        where TSrc : class
    {
        /// <summary>
        /// Destination type for a collection transformation from <typeparamref name="TSrc"/> to <typeparamref name="TDst"/>
        /// </summary>
        /// <typeparam name="TDst"></typeparam>
        /// <returns></returns>
        ICollection<TDst?>? To<TDst>() where TDst : class;
    }
}
