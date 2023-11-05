using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace SimpleAutoMapper
{
    internal interface IMappingDelegateRunner
    {
        object? Run(Delegate @delegate, object src);
        Func<object?, object?> AsFunc(Delegate @delegate);
    }

    internal static class MappingDelegateRunner
    {
        private static readonly ConcurrentDictionary<(Type SourceType, Type DestinationType), IMappingDelegateRunner> cache
            = new ConcurrentDictionary<(Type SourceType, Type DestinationType), IMappingDelegateRunner>();

        public static object? Run(Type sourceType, Type destinationType, Delegate @delegate, object src)
            => GetDelegateRunner(sourceType, destinationType).Run(@delegate, src);

        public static Func<object?, object?> GetFunc(Type sourceType, Type destinationType, Delegate @delegate)
            => GetDelegateRunner(sourceType, destinationType).AsFunc(@delegate);

        public static IMappingDelegateRunner GetDelegateRunner(Type sourceType, Type destinationType)
            => cache.GetOrAdd(
                (sourceType, destinationType),
                t => (IMappingDelegateRunner)Activator.CreateInstance(typeof(Runner<,>).MakeGenericType(sourceType, destinationType))
            );

        private sealed class Runner<TSrc, TDst> : IMappingDelegateRunner
            where TSrc : class
            where TDst : class
        {
            public Func<object?, object?> AsFunc(Delegate @delegate)
                => (src) => ((Func<TSrc?, TDst?>)@delegate)((TSrc?)src);

            public object? Run(Delegate @delegate, object src)
                => ((Func<TSrc?, TDst?>)@delegate)((TSrc?)src);
        }
    }
}
