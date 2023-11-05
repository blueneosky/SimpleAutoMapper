using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace SimpleAutoMapper
{
    internal interface ITypeMappingBuilder : IDisposable
    {
        ITypeMapping Mapping { get; }
        void Build();
    }

    internal static class TypeMappingBuilder
    {
        public static ITypeMappingBuilder Create(Type sourceType, Type destinationType, ILogger logger)
        {
            _ = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            _ = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
            if (!sourceType.IsClass) throw new ArgumentException("Must be a class type", nameof(sourceType));
            if (!destinationType.IsClass) throw new ArgumentException("Must be a class type", nameof(destinationType));
            if (destinationType.IsAbstract || destinationType.IsInterface)
                throw new ArgumentException("Must be a instantiable", nameof(destinationType));

            var builderType = typeof(TypeMappingBuilder<,>).MakeGenericType(sourceType, destinationType);
            return (ITypeMappingBuilder)Activator.CreateInstance(builderType, logger);
        }
    }

    internal class TypeMappingBuilder<TSrc, TDst> : ITypeMappingBuilder
        where TSrc : class
        where TDst : class
    {
        private readonly ReaderWriterLockSlim _mapperLock = new ReaderWriterLockSlim();
        private readonly ILogger _logger;
        private readonly TypeMapping<TSrc, TDst> _mapping;

        public TypeMappingBuilder(ILogger logger)
        {
            this._logger = logger;
            this._mapperLock.EnterWriteLock();
            this._mapping = new TypeMapping<TSrc, TDst>(this._mapperLock);
        }

        public ITypeMapping Mapping => this._mapping;

        public void Dispose()
        {
            _mapperLock.ExitWriteLock();
            while (_mapperLock.CurrentReadCount > 0 || _mapperLock.WaitingReadCount > 0)
                Thread.Sleep(0);    // context thread switch for helping other readers to release their lock
            ((IDisposable)_mapperLock).Dispose();
        }

        public void Build()
        {
            var srcType = this._mapping.SourceType;
            var dstType = this._mapping.DestinationType;

            var ctxtParameter = Expression.Parameter(typeof(IMappingContext), "ctxt");
            var srcParameter = Expression.Parameter(srcType, "src");

            var bindings = GetInitExprByDstElements()
                .Select(t => Expression.Bind(t.DstMemberInfo, t.InitExpr));
            Expression body = Expression.MemberInit(Expression.New(typeof(TDst)), bindings);
            while (body.CanReduce)
                body = body.ReduceAndCheck();
            var mappingExpr = Expression.Lambda<Func<IMappingContext, TSrc, TDst>>(body, ctxtParameter, srcParameter);

            try
            {
                var mapper = mappingExpr.Compile()
                    ?? throw new InvalidOperationException($"Compilation failed for mapping {srcType} -> {dstType}");
                this._mapping.SetMapper(mapper!, isOptimized: true); // TODO isOptimized
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Build fail for mapping {SourceType} -> {DestinationType}", srcType, dstType);
                var dispatchInfo = ExceptionDispatchInfo.Capture(ex);
                this._mapping.SetMapper((ctxt, src) => { dispatchInfo.Throw(); return default; }, true);    // will never work
            }

            return;

            //=============================================================

            IEnumerable<(MemberInfo DstMemberInfo, Expression InitExpr)> GetInitExprByDstElements()
            {
                var srcMemberByNames = GetSourceMembers().ToDictionary(m => m.Name);
                var dstMembers = GetDestinaitonMembers();

                foreach (var member in dstMembers)
                {
                    Expression initExpr = null;   // simpleAutoMapper.Config.GetPropMapping(srcType, dstType, member.Name, ctxtParameter, srcParameter);
                    if (initExpr != null)
                        yield return (member, initExpr);

                    if (!srcMemberByNames.TryGetValue(member.Name, out var srcMember))
                    {
                        this._logger.LogInformation(
                            "No source member found in {srcType} or mapping defined for {DestinationMemberName} of {DestinationType}",
                            srcType, member.Name, dstType);
                        continue;
                    }

                    initExpr = srcMember.MemberType switch
                    {
                        MemberTypes.Field => Expression.Field(srcParameter, ((FieldInfo)srcMember)),
                        MemberTypes.Property => Expression.Property(srcParameter, ((PropertyInfo)srcMember)),
                        _ => throw new InvalidOperationException($"unexpected member type {srcMember.MemberType}"),
                    };
                    yield return (member, initExpr);
                }
            }

            IEnumerable<MemberInfo> GetSourceMembers()
                => Enumerable.Concat<MemberInfo>(
                    srcType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(f => f.IsPublic && !f.IsStatic),
                    srcType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanRead && p.GetMethod.IsPublic && !p.GetMethod.IsAbstract && !p.GetMethod.IsStatic)
                    );

            IEnumerable<MemberInfo> GetDestinaitonMembers()
                => Enumerable.Concat<MemberInfo>(
                    dstType.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(f => f.IsPublic && !f.IsStatic /*&& !f.IsInitOnly*/),
                    dstType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.CanWrite && p.SetMethod.IsPublic && !p.SetMethod.IsAbstract && !p.SetMethod.IsStatic)
                    );
        }

    }
}
