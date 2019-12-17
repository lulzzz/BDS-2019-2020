using System;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;
using Orleans.Streams;
using Orleans;
using Orleans.Concurrency;

namespace GrainStreamProcessing.GrainImpl
{
    [Reentrant]
    public abstract class FlatMapGrain : Orleans.Grain, IFlatMapGrain, IFlatMapFunction
    {
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public abstract List<MyType> Apply(MyType e);

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

        public async Task Process(MyType e, StreamSequenceToken sequenceToken) // Implements the Process method from IFilter
        {
            List<MyType> result = new List<MyType>();
            if (e.value == "watermark") result.Add(e);
            else
            {
                string Key = await jobManager.GetKey(this.GetPrimaryKey());
                string Value = await jobManager.GetValue(this.GetPrimaryKey());
                MyType new_e = NewEvent.CreateNewEvent(e, Key, Value);
                result = Apply(new_e);
            }
            List<Task> t = new List<Task>();
            List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());
            foreach (var item1 in streams)
            {
                var stream = streamProvider.GetStream<MyType>(item1, null);
                foreach (var item2 in result) t.Add(stream.OnNextAsync(item2));
            }
            await Task.WhenAll(t);
        }
    }

    public class SplitValue : FlatMapGrain
    {
        public override List<MyType> Apply(MyType e)
        {
            string key = e.key;
            string value = e.value;
            Timestamp time = e.timestamp;

            List<MyType> strs = new List<MyType>();
            string[] parts = value.Split(" ");
            foreach (string part in parts)
            {
                MyType new_part = new MyType(key, part, time);
                strs.Add(new_part);
            }
            return strs;
        }
    }
}
