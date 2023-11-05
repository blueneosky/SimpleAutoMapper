using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace SimpleAutoMapper
{
    [DebuggerDisplay("{ToString()}")]
    internal class TypeMapping<TSrc, TDst> : ITypeMapping<TSrc, TDst>
        where TSrc : class
        where TDst : class
    {
        private volatile ReaderWriterLockSlim? _builderLock;

        private volatile Tuple<TypeMappingState, Func<IMappingContext, TSrc?, TDst?>> _mapping;

        public Type SourceType => typeof(TSrc);
        public Type DestinationType => typeof(TDst);

        public override string ToString() => $"Mapping from {SourceType} to {DestinationType} ({CurrentState})";

        public TypeMappingState CurrentState => this._mapping.Item1;

        public Func<IMappingContext, TSrc?, TDst?> Mapper => this._mapping.Item2;

        public TypeMapping(ReaderWriterLockSlim builderLock)
        {
            this._builderLock = builderLock;
            Func<IMappingContext, TSrc?, TDst?> notImplementedMapper = (ctx, src) => throw new NotImplementedException($"Mapper not yet compiled for {typeof(TSrc)} -> {typeof(TDst)}");
            this._mapping = Tuple.Create(TypeMappingState.NotImplemented, notImplementedMapper);
        }

        Delegate ITypeMapping.GetMapper(IMappingContext context) => this.GetMapper(context);

        public Func<TSrc?, TDst?> GetMapper(IMappingContext context)
        {
            WaitBuildEnd();
            return (src) => this.Mapper(context, src);
        }

        public void SetMapper(Func<IMappingContext, TSrc?, TDst?> mapper, bool isOptimized)
        {
            this._mapping = Tuple.Create(
                isOptimized ? TypeMappingState.Optimized : TypeMappingState.Implemented,
                mapper);
            if (isOptimized)
                this._builderLock = null;
        }

        public ITypeMapping WhenReady()
        {
            WaitBuildEnd();
            Debug.Assert(this.CurrentState > TypeMappingState.NotImplemented);
            return this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WaitBuildEnd()
        {
            var builderLock = this._builderLock;   // copy needed !
            if (builderLock != null)
            {
                builderLock.EnterReadLock();
                builderLock.ExitReadLock();
            }
        }
    }
}
