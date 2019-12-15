using System.Collections.Generic;

namespace StreamProcessing.Function
{
    public class WindowFunction
    {
        // long: the end time of a window
        public Dictionary<long, List<MyType>> window_data;
        public long window_length;
        public long window_slide;

        public WindowFunction(long length, long slide)
        {
            window_length = length;
            window_slide = slide;
        }

        public Dictionary<long, List<MyType>> FeedData(MyType e)
        {
            string value = e.value;
            long time = e.timestamp.getTimestamp();

            if (value == "watermark") return Trigger(time);
            
            long min_window = time / window_slide;

            // where the min_window starts and ends
            long end = (min_window + 1) * window_slide;
            long start = end - window_length;

            // add the record to all possible windows
            while (time >= start && time < end)
            {
                // assign new timestamps to the records after window operation
                e.timestamp.setTimestamp(end - 1);

                if (!window_data.ContainsKey(end)) window_data.Add(end, new List<MyType>());
                window_data[end].Add(e);

                start += window_slide;
                end += window_slide;
            }

            return null;
        }

        private Dictionary<long, List<MyType>> Trigger(long watermark)
        {
            Dictionary<long, List<MyType>> fired_window = null;
            foreach (KeyValuePair<long, List<MyType>> e in window_data)
            {
                long end = e.Key;
                if (watermark >= end)
                {
                    fired_window.Add(end, e.Value);
                    window_data.Remove(end);
                }
            }
            return fired_window;
        }
    }
}
