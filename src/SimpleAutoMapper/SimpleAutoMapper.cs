using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace SimpleAutoMapper
{
    public class SimpleAutoMapper : ISimpleAutoMapper
    {
        private readonly ILogger<SimpleAutoMapper> _logger;
        private readonly ConcurrentDictionary<(Type TSrc, Type TDst), ITypeMapping> _mappings = new ConcurrentDictionary<(Type TSrc, Type TDst), ITypeMapping>();

        public SimpleAutoMapper(ILogger<SimpleAutoMapper>? logger = null)
        {
            this._logger = logger ?? NullLoggerFactory.Instance.CreateLogger<SimpleAutoMapper>();
        }

        public ITypeMapping GetTypeMapping(Type sourceType, Type destinationType)
            => this.GetOrCreateMapping(sourceType, destinationType);

        public Delegate GetMapper(Type sourceType, Type destinationType)
            => this.GetOrCreateMapping(sourceType, destinationType).GetMapper(CreateContext());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal ITypeMapping GetOrCreateMapping(Type sourceType, Type destinationType)
        {
            // NOTE : GetOrAdd is not fully synchronized : the factory can be call concurrently,
            //        but only the first one is inserted, others are discarded
            return this._mappings.GetOrAdd((sourceType, destinationType), types => CreateMapping(types.TSrc, types.TDst));
        }

        private ITypeMapping CreateMapping(Type sourceType, Type destinationType)
        {
            using var builder = TypeMappingBuilder.Create(sourceType, destinationType, this._logger);
            var mapping = this._mappings.GetOrAdd((sourceType, destinationType), builder.Mapping);
            if (!object.ReferenceEquals(mapping, builder.Mapping))
                return mapping.WhenReady();

            builder.Build();

            return mapping;
        }

        private IMappingContext CreateContext() => new DefaultMappingContext();
    }
}
