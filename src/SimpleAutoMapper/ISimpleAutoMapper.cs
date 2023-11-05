using System;

namespace SimpleAutoMapper
{
    public interface ISimpleAutoMapper
    {
        /// <summary>
        /// Get the mapping definition.
        /// </summary>
        /// <returns></returns>
        ITypeMapping GetTypeMapping(Type sourceType, Type destinationType);

        /// <summary>
        /// Get the mapper delegate
        /// </summary>
        /// <returns></returns>
        Delegate GetMapper(Type sourceType, Type destinationType);
    }
}
