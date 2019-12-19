using Orleans;
using Orleans.Streams;
using StreamProcessing.Grain.Interface;
using System.Threading.Tasks;
using System;
using StreamProcessing.Function;
using System.Collections.Generic;
using Orleans.Concurrency;

namespace StreamProcessing.Grain.Implementation
{
    public class SourceGrain : Orleans.Grain, ISourceGrain
    {
        long delay;
        long currentMaxTimestamp;
        IJobManagerGrain jobManager;
        IStreamProvider streamProvider;

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            jobManager = GrainFactory.GetGrain<IJobManagerGrain>(0, "JobManager");

            delay = await jobManager.GetDelay();

            // ask the JobManager which streams it should subscribe
            var subscribe = await jobManager.GetSubscribe(this.GetPrimaryKey());

            foreach (var streamID in subscribe)
            {
                var stream = streamProvider.GetStream<string>(streamID, null);

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

        // reference: https://github.com/dotnet/orleans/blob/master/src/Orleans.Core.Abstractions/Streams/Core/StreamSequenceToken.cs
        private async Task Process(string message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"SourceGrain receives: {message}");
            string[] parts = message.Split(" ");
            string new_message = "";
            // new_message is without timestamp column
            for (int i = 0; i < parts.Length - 1; i++) new_message += parts[i] + " ";
            long time = Convert.ToInt64(parts[parts.Length - 1]);
            Timestamp timestamp = new Timestamp(time);

            // reference: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
            var record = new MyType("", new_message, timestamp);
            bool emit_watermark = false;
            MyType watermark = new MyType("", "watermark", new Timestamp(-1));

            // see if need to emit watermark
            if (time > currentMaxTimestamp)
            {
                currentMaxTimestamp = time;
                watermark.timestamp.SetTimestamp(currentMaxTimestamp - delay);  // important !!!!!!
                emit_watermark = true;
            }

            // ask the Job Manager what is the guid for the stream, and publish the data to some streams
            List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());
            List<Task> t = new List<Task>();
            foreach (var item in streams)
            {
                var stream = streamProvider.GetStream<MyType>(item, null);
                t.Add(stream.OnNextAsync(record));
                if (emit_watermark) t.Add(stream.OnNextAsync(watermark));
            }
            await Task.WhenAll(t);
        }
    }
}
