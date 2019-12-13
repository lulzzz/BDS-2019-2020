using System;
using StreamProcessing.Function;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;
using System.Collections.Generic;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class WindowJoinGrain<T> : Orleans.Grain, IJoinGrain, IWindowFunction<T>
    {
        public Dictionary<int, String> windows = null;
        public long window_length = 0;
        public long window_slide = 0;

        public abstract List<T> Apply(T e);    // Implement the Apply method from IWindowFunction
        public List<T> Trigger(int watermark)
        {
            return null;
        }

        public Task Process(object e1, object e2)   // Implement the Process method from IJoinGrain
        {
            // TODO: ask job manager how to set window length and slide
            List<T> result1 = Apply((T)e1);
            List<T> result2 = Apply((T)e2);
            this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(result1);
            return Task.CompletedTask;
        }
    }

    public class JoinOnKey : WindowJoinGrain<String>
    {
        public override List<String> Apply(String e)
        {
            List<String> fired_windows = null;

            String[] parts = e.Split(" ");

            // when receive a watermark, fire corresponding windows if needed
            if (parts[0] == "watermark")
            {
                int watermark = Convert.ToInt32(parts[0]);
                fired_windows = Trigger(watermark);
            }

            // otherwise, it's a normal record
            long timestamp = Convert.ToInt64(parts[parts.Length - 1]);
            long first_window = timestamp / window_slide;     // the first window that this record should be put in

            return fired_windows;
        }

        public List<String> Trigger(int watermark)
        {

            return null;
        }
    }
}
