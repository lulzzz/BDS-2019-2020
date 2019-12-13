using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class WindowJoinGrain : Orleans.Grain, IJoinGrain
    {
        public Dictionary<long, List<MyType>> data1;
        public Dictionary<long, List<MyType>> data2;

        public Task Process(MyType e1, MyType e2)   // Implement the Process method from IJoinGrain
        {
            // TODO: ask job manager how to set window length and slide
            long window_length = 15000;
            long window_slide = 5000;
            WindowFunction func = new WindowFunction(window_length, window_slide);

            // r1 and r2 could be null
            var r1 = func.FeedData(e1);
            var r2 = func.FeedData(e2);

            foreach (KeyValuePair<long, List<MyType>> e in r1) data1.Add(e.Key, e.Value);
            foreach (KeyValuePair<long, List<MyType>> e in r2) data2.Add(e.Key, e.Value);

            foreach (KeyValuePair<long, List<MyType>> e in data1)
            {
                if (data2.ContainsKey(e.Key))
                {
                    Join(e.Value, data2[e.Key]);
                    data1.Remove(e.Key);
                    data2.Remove(e.Key);
                }
            }

            return Task.CompletedTask;
        }

        private void Join(List<MyType> input1, List<MyType> input2)
        {
            foreach (MyType r1 in input1)
            {
                foreach (MyType r2 in input2)
                {
                    if (r1.key == r2.key)
                    {
                        MyType r = new MyType(r1.key, r1.value + " " + r2.value, r1.timestamp);
                        this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(r);
                    }
                }
            }
        }
    }
}
