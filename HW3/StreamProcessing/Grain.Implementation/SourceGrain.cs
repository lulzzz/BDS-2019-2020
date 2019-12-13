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

            //Add your logic here to process received data
            String[] parts = message.Split(" ");
            long time = Convert.ToInt64(parts[parts.Length - 1]);
            Timestamp timestamp = new Timestamp(time);

            // reference: https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/using-structs
            var record = new MyType("", message, timestamp);

            this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(record);

            return Task.CompletedTask;
        }
    }
}
