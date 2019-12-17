using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;
using Orleans.Streams;
using Orleans;
using System;
using Orleans.Concurrency;

namespace StreamProcessing.Grain.Implementation
{
    [Reentrant]
    public class WindowJoinGrain : Orleans.Grain, IJoinGrain
    {
        private long window_length;
        private long window_slide;
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;
        public Dictionary<long, List<MyType>> data1;
        public Dictionary<long, List<MyType>> data2;
        private WindowFunction func1;
        private WindowFunction func2;

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            List<Task> t = new List<Task>();
            
            streamProvider = GetStreamProvider("SMSProvider");
            jobManager = GrainFactory.GetGrain<IJobManagerGrain>(0, "JobManager");
            var window = await jobManager.GetWindow(this.GetPrimaryKey());
            window_length = window.Item1;
            window_slide = window.Item2;
            data1 = new Dictionary<long, List<MyType>>();
            data2 = new Dictionary<long, List<MyType>>();
            func1 = new WindowFunction(window_length, window_slide);
            func2 = new WindowFunction(window_length, window_slide);

            // ask the JobManager which streams it should subscribe
            var subscribe = await jobManager.GetSubscribe(this.GetPrimaryKey());

            /********** Handle the first source stream*************************/
            var stream1 = streamProvider.GetStream<MyType>(subscribe[0], null);
            var subscriptionHandles = await stream1.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(Process1);
            }
            t.Add(stream1.SubscribeAsync(Process1));

            /********** Handle the second source stream************************/
            var stream2 = streamProvider.GetStream<MyType>(subscribe[1], null);
            var subscriptionHandles2 = await stream2.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(Process2);
            }
            t.Add(stream1.SubscribeAsync(Process2));

            await Task.WhenAll(t);
        }

        public async Task Process1(MyType e, StreamSequenceToken sequenceToken)   // Implement the Process method from IJoinGrain
        {
            //Console.WriteLine($"Source data 1 receives: {e.key}, {e.value}, {e.timestamp.GetTimestamp()}");

            MyType new_e;
            if (e.value == "watermark") new_e = e;
            else
            {
                var key = await jobManager.GetKey(this.GetPrimaryKey());
                var value = await jobManager.GetValue(this.GetPrimaryKey());
                string Key = key.Split(",")[0];
                string Value = value.Split(",")[0];
                new_e = NewEvent.CreateNewEvent(e, Key, Value);
            }

            var r = func1.FeedData(new_e);   // r could be null

            if (r.Count > 0)
            {
                foreach (KeyValuePair<long, List<MyType>> ee in r) data1.Add(ee.Key, ee.Value);
                await CheckIfCanJoin();
            }
            else await Task.CompletedTask;
        }

        public async Task Process2(MyType e, StreamSequenceToken sequenceToken)   // Implement the Process method from IJoinGrain
        {
            //Console.WriteLine($"Source data 2 receives: {e.key}, {e.value}, {e.timestamp.GetTimestamp()}");

            MyType new_e;
            if (e.value == "watermark") new_e = e;
            else
            {
                var key = await jobManager.GetKey(this.GetPrimaryKey());
                var value = await jobManager.GetValue(this.GetPrimaryKey());
                string Key = key.Split(",")[1];
                string Value = value.Split(",")[1];
                new_e = NewEvent.CreateNewEvent(e, Key, Value);
            }

            var r = func2.FeedData(new_e);   // r could be null

            if (r.Count > 0)
            {
                foreach (KeyValuePair<long, List<MyType>> ee in r) data2.Add(ee.Key, ee.Value);
                await CheckIfCanJoin();
            }
            else await Task.CompletedTask;
        }

        private async Task CheckIfCanJoin()
        {
            foreach (KeyValuePair<long, List<MyType>> ee in data1)
            {
                if (data2.ContainsKey(ee.Key))
                {
                    await Join(ee.Value, data2[ee.Key]);
                    data1.Remove(ee.Key);
                    data2.Remove(ee.Key);
                }
            }
        }

        private async Task Join(List<MyType> input1, List<MyType> input2)
        {
            List<Task> t = new List<Task>();
            foreach (MyType r1 in input1)
            {
                foreach (MyType r2 in input2)
                {
                    if (r1.key == r2.key)
                    {
                        MyType r = new MyType(r1.key, r1.value + " " + r2.value, r1.timestamp);
                        List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());
                        //Console.WriteLine($"WindowJoinGrain output: {r.key}, {r.value}");
                        foreach (var item in streams)
                        {
                            var stream = streamProvider.GetStream<MyType>(item, null);
                            t.Add(stream.OnNextAsync(r));
                        }
                    }
                }
            }
            await Task.WhenAll(t);
        }
    }
}
