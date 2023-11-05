using System;

namespace SimpleAutoMapper
{
    public enum TypeMappingState
    {
        None = 0,
        NotImplemented,
        Implemented,
        Optimized,
    }

    /// <summary>
    /// Genericless information of a mapping.
    /// </summary>
    public interface ITypeMapping
    {
        /// <summary>
        /// Source type of this mapping
        /// </summary>
        Type SourceType { get; }

        /// <summary>
        /// Destination type of this mapping
        /// </summary>
        Type DestinationType { get; }

        /// <summary>
        /// Get the typeless delegate use to map src-&gt;dst.
        /// Expected : Func&lt;in TSrc?, out TDst?&gt;
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Delegate GetMapper(IMappingContext context);

        /// <summary>
        /// Get the current state of the mapping
        /// </summary>
        TypeMappingState CurrentState { get; }

        /// <summary>
        /// Wait until the <see cref="CurrentState"/> is greater than <see cref="TypeMappingState.NotImplemented"/>
        /// </summary>
        /// <returns></returns>
        ITypeMapping WhenReady();
    }

    /// <summary>
    /// Information of a mapping.
    /// </summary>
    /// <typeparam name="TSrc"></typeparam>
    /// <typeparam name="TDst"></typeparam>
    public interface ITypeMapping<TSrc, TDst> : ITypeMapping
        where TSrc : class
        where TDst : class
    {
        /// <summary>
        /// Mapper delegate
        /// </summary>
        public Func<IMappingContext, TSrc?, TDst?> Mapper { get; }

        /// <summary>
        /// Get the typeless delegate use to map src-&gt;dst.
        /// Expected : Func&lt;in TSrc?, out TDst?&gt;
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public new Func<TSrc?, TDst?> GetMapper(IMappingContext context);
    }
}
