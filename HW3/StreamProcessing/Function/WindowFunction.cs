using System.Collections.Generic;

namespace StreamProcessing.Function
{
    public class WindowFunction
    {
        // long: the end time of a window
        public Dictionary<long, MyType> window_data = null;
        public long window_length;
        public long window_slide;

        public WindowFunction(long length, long slide)
        {
            window_length = length;
            window_slide = slide;
        }

        public List<MyType> FeedData(MyType e)
        {
            List<MyType> fired_window = null;
            string value = e.value;
            long time = e.timestamp.getTimestamp();

            if (value == "watermark") fired_window = Trigger(time);
            else
            {
                long min_window = time / window_slide;

                // where the min_window starts and ends
                long end = (min_window + 1) * window_slide;
                long start = end - window_length;

                // add the record to all possible windows
                while (time >= start && time < end)
                {
                    // assign new timestamps to the records after window operation
                    e.timestamp.setTimestamp(end - 1);
                    window_data.Add(end, e);
                    start += window_slide;
                    end += window_slide;
                }
            }

            return fired_window;
        }

        private List<MyType> Trigger(long watermark)
        {
            List<MyType> fired_window = null;
            foreach (KeyValuePair<long, MyType> e in window_data)
            {
                long end = e.Key;
                if (watermark >= end)
                {
                    fired_window.Add(e.Value);
                    window_data.Remove(end);
                }
            }
            return fired_window;
        }
    }
}
