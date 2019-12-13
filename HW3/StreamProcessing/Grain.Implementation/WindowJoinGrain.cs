using System;
using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class WindowJoinGrain : Orleans.Grain, IJoinGrain
    {
        public Task Process(MyType e1, MyType e2)   // Implement the Process method from IJoinGrain
        {
            // TODO: ask job manager how to set window length and slide
            long window_length = 15000;
            long window_slide = 5000;
            WindowFunction func = new WindowFunction(window_length, window_slide);
            List<MyType> r1 = func.FeedData(e1);
            List<MyType> r2 = func.FeedData(e2);
            // ??????????????
            foreach (MyType r in ) ;
            this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(result1);
            return Task.CompletedTask;
        }
    }
}
