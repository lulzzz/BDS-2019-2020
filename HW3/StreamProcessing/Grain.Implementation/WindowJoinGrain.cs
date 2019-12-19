using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;
using Orleans.Streams;
using Orleans;
using System;

namespace StreamProcessing.Grain.Implementation
{
    public class WindowJoinGrain : Orleans.Grain, IJoinGrain
    {
        private long window_length;
        private long window_slide;
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;
        public SortedList<long, List<MyType>> data1;
        public SortedList<long, List<MyType>> data2;
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
            data1 = new SortedList<long, List<MyType>>();
            data2 = new SortedList<long, List<MyType>>();
            func1 = new WindowFunction(window_length, window_slide);
            func2 = new WindowFunction(window_length, window_slide);

            // ask the JobManager which streams it should subscribe
            var subscribe = await jobManager.GetTwoSourceSubscribe(this.GetPrimaryKey());

            /********** Handle the first source stream*************************/
            var stream1 = streamProvider.GetStream<MyType>(subscribe.Item1, null);
            var subscriptionHandles1 = await stream1.GetAllSubscriptionHandles();
            if (subscriptionHandles1.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles1)
                    await subscriptionHandle.ResumeAsync(Process1);
            }
            t.Add(stream1.SubscribeAsync(Process1));

            /********** Handle the second source stream************************/
            var stream2 = streamProvider.GetStream<MyType>(subscribe.Item2, null);
            var subscriptionHandles2 = await stream2.GetAllSubscriptionHandles();
            if (subscriptionHandles2.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles2)
                    await subscriptionHandle.ResumeAsync(Process2);
            }
            t.Add(stream2.SubscribeAsync(Process2));

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

            var r = func1.FeedData(new_e);   // r could be an empty list
            if (r.Count > 0)
            {
                foreach (KeyValuePair<long, List<MyType>> ee in r)
                {
                    if (!data1.ContainsKey(ee.Key)) data1.Add(ee.Key, ee.Value);
                    else throw new Exception($"Exception: data1 already has the key {ee.Key}");
                }
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
                foreach (KeyValuePair<long, List<MyType>> ee in r)
                {
                    if (!data2.ContainsKey(ee.Key)) data2.Add(ee.Key, ee.Value);
                    else throw new Exception($"Exception: data2 already has the key {ee.Key}");
                }
                await CheckIfCanJoin();

            }
            else await Task.CompletedTask;
        }

        private async Task CheckIfCanJoin()
        {
            var removed_key = new List<long>();
            foreach (KeyValuePair<long, List<MyType>> ee in data1)
            {
                if (data2.ContainsKey(ee.Key))
                {
                    await Join(ee.Value, data2[ee.Key]);
                    removed_key.Add(ee.Key);
                }
            }
            // delete the data that already finish join
            foreach (var key in removed_key)
            {
                data1.Remove(key);
                data2.Remove(key);
            }
        }

        private async Task Join(List<MyType> input1, List<MyType> input2)
        {
            Timestamp time = new Timestamp(0);
            List<Task> t = new List<Task>();
            List<Guid> streams = await jobManager.GetPublish(this.GetPrimaryKey());

            foreach (MyType r1 in input1)
            {
                foreach (MyType r2 in input2)
                {
                    if (r1.key == r2.key)
                    {
                        MyType r = new MyType(r1.key, r1.value + " " + r2.value, r1.timestamp);
                        foreach (var item in streams)
                        {
                            var stream = streamProvider.GetStream<MyType>(item, null);
                            t.Add(stream.OnNextAsync(r));
                        }
                    }
                }
                time = r1.timestamp;
            }

            // send a watermark after sending all result events for a window
            MyType watermark = new MyType("", "watermark", time);
            foreach (var item in streams)
            {
                var stream = streamProvider.GetStream<MyType>(item, null);
                t.Add(stream.OnNextAsync(watermark));
            }
            await Task.WhenAll(t);
        }
    }
}
