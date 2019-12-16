using Orleans;
using Orleans.Streams;
using StreamProcessing.Grain.Interface;
using System.Threading.Tasks;
using System;
using StreamProcessing.Function;
using System.Collections.Generic;

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

            delay = jobManager.GetDelay().Result;

            // ask the JobManager which streams it should subscribe
            var subscribe = jobManager.GetSubscribe(this.GetPrimaryKey()).Result;

            foreach (var streamID in subscribe)
            {
                var stream = streamProvider.GetStream<String>(streamID, "");

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
            string[] parts = message.Split(" ");
            string new_message = "";
            // new_message is without timestamp column
            for (int i = 0; i < parts.Length - 1; i++) new_message += parts[i] + " ";
            long time = Convert.ToInt64(parts[parts.Length - 1]);
            Timestamp timestamp = new Timestamp(time);

            // reference: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
            var record = new MyType("", new_message, timestamp);
            bool emit_watermark = false;
            MyType watermark = new MyType("", "", new Timestamp(-1));

            // see if need to emit watermark
            if (time > currentMaxTimestamp)
            {
                currentMaxTimestamp = time;
                Timestamp time_for_watermark = new Timestamp(currentMaxTimestamp - delay);
                watermark = new MyType("", "watermark", time_for_watermark);
                emit_watermark = true;
            }

            // ask the Job Manager what is the guid for the stream, and publish the data to some streams
            List<Guid> streams = jobManager.GetPublish(this.GetPrimaryKey()).Result;
            foreach (var item in streams)
            {
                var stream = streamProvider.GetStream<MyType>(item, "");
                await stream.OnNextAsync(record);
                if (emit_watermark) await stream.OnNextAsync(watermark);
            }
        }
    }
}
