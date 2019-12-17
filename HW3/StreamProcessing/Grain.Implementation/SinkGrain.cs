using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;


namespace StreamProcessing.Grain.Implementation
{
    [Reentrant]
    public class SinkGrain : Orleans.Grain, ISinkGrain
    {
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;

        public Task Init()
        {
            return Task.CompletedTask;
        }

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
            Console.WriteLine($"SinkGrain  key = {e.key}, value = {e.value}, {e.timestamp.GetTimestamp()}");     // Output received data to Silo console
            await Task.CompletedTask;
        }
    }
}
