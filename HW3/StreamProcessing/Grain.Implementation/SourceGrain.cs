using Orleans;
using Orleans.Streams;
using StreamProcessing.Grain.Interface;
using System.Threading.Tasks;
using System;
using StreamProcessing.Function;

namespace StreamProcessing.Grain.Implementation
{
    public class SourceGrain : Orleans.Grain, ISourceGrain
    {
        long delay = 0;
        long currentMaxTimestamp = 0;
        string streamName;
        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream {streamName} starts.");
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            var primaryKey = this.GetPrimaryKey(out streamName);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<String>(primaryKey, streamName);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();

            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                {
                    await subscriptionHandle.ResumeAsync(OnNextMessage);
                }
            }

            await stream.SubscribeAsync(OnNextMessage);
        }

        // reference: https://github.com/dotnet/orleans/blob/master/src/Orleans.Core.Abstractions/Streams/Core/StreamSequenceToken.cs
        private Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"Stream {streamName} receives: {message}.");
            
            String[] parts = message.Split(" ");
            String new_message = "";
            // new_message is without timestamp column
            for (int i = 0; i < parts.Length - 1; i++) new_message += " " + parts[i];
            long time = Convert.ToInt64(parts[parts.Length - 1]);
            Timestamp timestamp = new Timestamp(time);

            // reference: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
            var record = new MyType("", new_message, timestamp);

            // TODO: ask the Job Manager what is the next operator
            this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(record);

            // emit watermark
            if (time > currentMaxTimestamp)
            {
                currentMaxTimestamp = time;
                // TODO: client should set the delay, otherwise default delay is 0
                Timestamp time_for_watermark = new Timestamp(currentMaxTimestamp - delay);
                var watermark = new MyType("", "watermark", time_for_watermark);
                this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(watermark);
            }

            return Task.CompletedTask;
        }
    }
}
