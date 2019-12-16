using System;
using Orleans;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;
using Orleans.Streams;
using System.Collections.Generic;

namespace StreamProcessing.Grain.Implementation
{
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
            var subscribe = jobManager.GetSubscribe(this.GetPrimaryKey()).Result;

            foreach (var streamID in subscribe)
            {
                var stream = streamProvider.GetStream<MyType>(streamID, "");

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
            string Key = jobManager.GetKey(this.GetPrimaryKey()).Result;
            string Value = jobManager.GetValue(this.GetPrimaryKey()).Result;
            MyType new_e = NewEvent.CreateNewEvent(e, Key, Value);

            if (Apply(new_e)) // If the function returns true, send the element to SinkGrain
            {
                List<Guid> streams = jobManager.GetPublish(this.GetPrimaryKey()).Result;
                foreach (var item in streams)
                {
                    var stream = streamProvider.GetStream<MyType>(item, "");
                    await stream.OnNextAsync(new_e);
                }
            }
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
