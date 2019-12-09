using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Functions.Interfaces;

namespace Grains.Implementations
{
    public class SampleGrain : Orleans.Grain, ISample
    {
        object func;
        Guid sinkId;
        public Task Init<T>(Guid sinkId, IFilterFunction<T> func)
        {
            this.func = func;
            this.sinkId = sinkId;
            return Task.CompletedTask;
        }

        public Task Process<T>(T e)
        {
            IFilterFunction<T> function = (IFilterFunction<T>)func;
            if (function.Apply(e))
            {
                this.GrainFactory.GetGrain<ISink>(sinkId).Process<T>(e);
            }
            return Task.CompletedTask;
        }
    }
}