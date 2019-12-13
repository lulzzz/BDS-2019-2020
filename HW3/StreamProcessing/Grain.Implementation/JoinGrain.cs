using System;
using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class JoinGrain<T> : Orleans.Grain, IJoinGrain, IWindowFunction<T>
    {
        public abstract T Apply(T e1, T e2);
        public Task Process(object e1, object e2)   // Implements the Process method from IJoinGrain
        {
            T result = Apply((T)e1, (T)e2);
            this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(result);
            return Task.CompletedTask;
        }
    }
}
