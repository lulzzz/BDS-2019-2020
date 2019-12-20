using System;
using System.Collections.Generic;

namespace StreamProcessing.Function
{
    public class WindowFunction
    {
        // long: the end time of a window
        private SortedList<long, List<MyType>> window_data;
        private long window_length;
        private long window_slide;
        private long fired_max_window;

        public WindowFunction(long length, long slide)
        {
            window_length = length;
            window_slide = slide;
            fired_max_window = 0;
            window_data = new SortedList<long, List<MyType>>();
        }

        public SortedList<long, List<MyType>> FeedData(MyType e)
        {
            string value = e.value;
            long time = e.timestamp.GetTimestamp();
            var fired_window = new SortedList<long, List<MyType>>();
            //Console.WriteLine($"Get feeded data: {e.key}, {e.value}, {e.timestamp.GetTimestamp()}");
            if (value == "watermark")
            {
                fired_window = Trigger(time);
                foreach (var item in fired_window) window_data.Remove(item.Key);
            }
            else
            {
                long min_window = time / window_slide;

                // where the min_window starts and ends
                long end = (min_window + 1) * window_slide;
                long start = end - window_length;
                //Console.WriteLine("111111111111111");
                //if (fired_max_window >= end) throw new Exception($"Exception: the window {end} is already fired.");

                MyType new_e;
                //Console.WriteLine("222222222222222");
                // add the record to all possible windows
                while (time >= start && time < end)
                {
                    // assign new timestamps to the records after window operation
                    new_e = new MyType(e.key, e.value, new Timestamp(end - 1));

                    if (!window_data.ContainsKey(end)) window_data.Add(end, new List<MyType>());
                    window_data[end].Add(new_e);

                    start += window_slide;
                    end += window_slide;
                }
            }
            return fired_window;
        }

        private SortedList<long, List<MyType>> Trigger(long watermark)
        {
            var fired_window = new SortedList<long, List<MyType>>();
            foreach (KeyValuePair<long, List<MyType>> e in window_data)
            {
                long end = e.Key;
                if (watermark >= end) fired_window.Add(end, e.Value);
                fired_max_window = Math.Max(fired_max_window, e.Key);
            }
            return fired_window;
        }
    }
}
