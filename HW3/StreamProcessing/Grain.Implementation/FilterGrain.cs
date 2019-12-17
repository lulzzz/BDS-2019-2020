using System;
using Orleans;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;
using Orleans.Streams;
using System.Collections.Generic;
using Orleans.Concurrency;

namespace StreamProcessing.Grain.Implementation
{
    [Reentrant]
    public abstract class FilterGrain : Orleans.Grain, IFilterGrain, IFilterFunction
    {
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public abstract bool Apply(MyType e);

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            jobManager = GrainFactory.GetGrain<IJobManagerGrain>(0, "JobManager");

            // ask the JobManager which streams it should subscribe
            var subscribe = await jobManager.GetSubscribe(this.GetPrimaryKey());

            foreach (var streamID in subscribe)
            {
                var stream = streamProvider.GetStream<MyType>(streamID, null);

                // To resume stream in case of stream deactivation
                var subscriptionHandles = await stream.GetAllSubscriptionHandles();
                if (subscriptionHandles.Count > 0)
                {
                    foreach (var subscriptionHandle in subscriptionHandles)
                    {
                        await subscriptionHandle.ResumeAsync(Process);
                    }
                }

                // explicitly subscribe to a stream
                await stream.SubscribeAsync(Process);
            }
        }

        private async Task Process(MyType e, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"Filter Grain receives: {e.key}, {e.value}, {e.timestamp.GetTimestamp()}");
            bool emit_e;
            MyType ee;
            if (e.value == "watermark")
            {
                ee = e;
                emit_e = true;
            }
            else
            {
                string Key = await jobManager.GetKey(this.GetPrimaryKey());
                string Value = await jobManager.GetValue(this.GetPrimaryKey());
                ee = NewEvent.CreateNewEvent(e, Key, Value);
                emit_e = Apply(ee);
            }

            List<Task> t = new List<Task>();
            if (emit_e) // If the function returns true, send the element to SinkGrain
            {
                List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());
                foreach (var item in streams)
                {
                    var stream = streamProvider.GetStream<MyType>(item, null);
                    t.Add(stream.OnNextAsync(ee));
                }
            }
            await Task.WhenAll(t);
        }
    }
    
    public class LargerThanTenFilter : FilterGrain
    {
        // Implements the Apply method, filtering numbers larger than 10
        public override bool Apply(MyType e)
        {
            int value = Convert.ToInt32(e.key);
            if (value > 10) return true;
            return false;
        }
    }

    public class OddNumberFilter : FilterGrain
    {
        // Implements the Apply method, filtering odd numbers
        public override bool Apply(MyType e) 
        {
            int value = Convert.ToInt32(e.key);
            if (value % 2 == 1) return true;
            return false;
        }
    }
}
