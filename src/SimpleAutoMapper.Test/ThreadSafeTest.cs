using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAutoMapper.Test;

[TestClass]
public class ThreadSafeTest
{
    [TestMethod]
    public void ConcurrentMappingCreation()
    {
        if (Environment.ProcessorCount <= 4)
            Assert.Inconclusive("This test require at least 4 processors to run properly");

        const int constSpinIterationCount = 30000;
        var threadCount = Environment.ProcessorCount;
        using var barrier = new Barrier(threadCount);
        using var endGate = new Barrier(threadCount);
        var mapper = new SimpleAutoMapper();

        // setup concurrent thread context
        if (ThreadPool.ThreadCount < threadCount)
        {
            ThreadPool.GetMinThreads(out _, out var minIOC);
            if (!ThreadPool.SetMinThreads(threadCount, minIOC))
                Assert.Inconclusive($"Fail to set Min thread allocation to {threadCount}");
            while (ThreadPool.ThreadCount < threadCount)
                ThreadPool.QueueUserWorkItem(_ => Thread.Sleep(1));
            while (ThreadPool.PendingWorkItemCount > 0)
                Thread.Sleep(0);
        }
        if (ThreadPool.ThreadCount < threadCount)
            Assert.Inconclusive("Thread initialization failed");

        var objMappers = new ConcurrentBag<ITypeMapping>();

        for (int i = 1; i < threadCount; i++)
            Assert.IsTrue(ThreadPool.QueueUserWorkItem(GetMapping, i, false));

        var swSrcToDst = Stopwatch.StartNew();

        Debug.WriteLine($"continue on thread idx 0");
        barrier.SignalAndWait();
        Thread.SpinWait(constSpinIterationCount);

        var swDstToDst = Stopwatch.StartNew();
        var objMapper = mapper.GetMapper(typeof(ScalarDst), typeof(ScalarDst));
        swDstToDst.Stop();

        Assert.IsTrue(endGate.SignalAndWait(TimeSpan.FromSeconds(5)), "End gate time out");
        swSrcToDst.Stop();

        Debug.WriteLine($"concurrent mapping ({threadCount - 1} threads) : {swSrcToDst.ElapsedMilliseconds} ms");
        Debug.WriteLine($"with a parallel mapping : {swDstToDst.ElapsedMilliseconds} ms");

        Assert.AreEqual(1, objMappers.Distinct().Count());

        Assert.IsTrue(swDstToDst.Elapsed <= 0.90 * swSrcToDst.Elapsed);

        //=============================================

        void GetMapping(int threadIdx)
        {
            Debug.WriteLine($"start of thread idx {threadIdx}");

            barrier.SignalAndWait();
            Thread.SpinWait(constSpinIterationCount);
            var typeMapping = mapper.GetTypeMapping(typeof(ScalarSrc), typeof(ScalarDst));
            var objMapper = typeMapping.GetMapper(null!);
            objMappers.Add(typeMapping);

            endGate.SignalAndWait();
        }
    }
}
