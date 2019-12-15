using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;
using Orleans.Streams;
using Orleans;
using System;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class WindowJoinGrain : Orleans.Grain, IJoinGrain
    {
        private long window_length;
        private long window_slide;
        private IJobManagerGrain jobManager;
        private IStreamProvider streamProvider;
        public Dictionary<long, List<MyType>> data1;
        public Dictionary<long, List<MyType>> data2;

        public override async Task OnActivateAsync()
        {
            List<Task> t = null;
            
            streamProvider = GetStreamProvider("SMSProvider");
            jobManager = GrainFactory.GetGrain<IJobManagerGrain>(0, "JobManager");
            window_length = jobManager.getWindow(this.GetPrimaryKey()).Item1;
            window_slide = jobManager.getWindow(this.GetPrimaryKey()).Item2;

            // ask the JobManager which streams it should subscribe
            var subscribe = jobManager.getSubscribe(this.GetPrimaryKey());

            /********** Handle the first source stream*************************/
            var stream1 = streamProvider.GetStream<MyType>(subscribe[0], "");
            var subscriptionHandles = await stream1.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(Process1);
            }
            t.Add(stream1.SubscribeAsync(Process1));

            /********** Handle the second source stream************************/
            var stream2 = streamProvider.GetStream<MyType>(subscribe[1], "");
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
            String Key = jobManager.getKey(this.GetPrimaryKey()).Split(",")[0];
            String Value = jobManager.getValue(this.GetPrimaryKey()).Split(",")[0];
            MyType new_e = NewEvent.CreateNewEvent(e, Key, Value);

            WindowFunction func = new WindowFunction(window_length, window_slide);

            var r = func.FeedData(new_e);   // r could be null

            foreach (KeyValuePair<long, List<MyType>> ee in r) data1.Add(ee.Key, ee.Value);
            await checkIfCanJoin();
        }

        public async Task Process2(MyType e, StreamSequenceToken sequenceToken)   // Implement the Process method from IJoinGrain
        {
            String Key = jobManager.getKey(this.GetPrimaryKey()).Split(",")[1];
            String Value = jobManager.getValue(this.GetPrimaryKey()).Split(",")[1];
            MyType new_e = NewEvent.CreateNewEvent(e, Key, Value);

            WindowFunction func = new WindowFunction(window_length, window_slide);

            var r = func.FeedData(new_e);   // r could be null

            foreach (KeyValuePair<long, List<MyType>> ee in r) data2.Add(ee.Key, ee.Value);
            await checkIfCanJoin();
        }

        private async Task checkIfCanJoin()
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
            foreach (MyType r1 in input1)
            {
                foreach (MyType r2 in input2)
                {
                    if (r1.key == r2.key)
                    {
                        MyType r = new MyType(r1.key, r1.value + " " + r2.value, r1.timestamp);
                        List<Guid> streams = jobManager.getPublish(this.GetPrimaryKey());
                        foreach (var item in streams)
                        {
                            var stream = streamProvider.GetStream<MyType>(item, "");
                            await stream.OnNextAsync(r);
                        }
                    }
                }
            }
        }
    }
}
