using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orleans;
using Orleans.Streams;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class WindowAggregateGrain : Orleans.Grain, IWindowAggregateGrain
    {
        private long window_length;
        private long window_slide;
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;
        private WindowFunction func;

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public abstract List<MyType> Apply(List<MyType> records);

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            jobManager = GrainFactory.GetGrain<IJobManagerGrain>(0, "JobManager");
            var window = await jobManager.GetWindow(this.GetPrimaryKey());
            window_length = window.Item1;
            window_slide = window.Item2;
            func = new WindowFunction(window_length, window_slide);

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
            List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());
            List<Task> t = new List<Task>();
            
            MyType ee;
            if (e.value == "watermark") ee = e;
            else
            {
                string Key = await jobManager.GetKey(this.GetPrimaryKey());
                string Value = await jobManager.GetValue(this.GetPrimaryKey());
                ee = NewEvent.CreateNewEvent(e, Key, Value);
            }

            var r = func.FeedData(ee);   // r could be an empty dictionary

            if (r.Count > 0)
            {
                foreach (KeyValuePair<long, List<MyType>> records in r)
                {
                    MyType watermark = new MyType("", "watermark", new Timestamp(records.Key - 1)); // important!!!!!
                    List<MyType> result = Apply(records.Value);
                    foreach (var item in streams)
                    {
                        var stream = streamProvider.GetStream<MyType>(item, null);
                        foreach (var record in result)
                        {
                            t.Add(stream.OnNextAsync(record));
                        }
                        t.Add(stream.OnNextAsync(watermark));
                    }
                }
                await Task.WhenAll(t);
            }
            else await Task.CompletedTask;
        }
    }

    public class WindowDuplicateRemover : WindowAggregateGrain
    {
        public override List<MyType> Apply(List<MyType> records)
        {
            return records.Distinct().ToList();
        }
    }

    public class WindowCountByKey : WindowAggregateGrain
    {
        public override List<MyType> Apply(List<MyType> records)
        {
            Timestamp t = records[0].timestamp;
            // string: the key, int: the count
            var result = new Dictionary<string, int>();
            foreach (var item in records)
            {
                if (result.ContainsKey(item.key)) result[item.key] += 1;
                else result.Add(item.key, 1);
            }

            var resultList = new List<MyType>();
            foreach (var item in result)
            {
                MyType e = new MyType(item.Key, item.Value.ToString(), t);
                resultList.Add(e);
            }
            return resultList;
        }
    }
}
