using System;

namespace StreamProcessing.Function
{
    [Serializable]
    public class Timestamp
    {
        long timestamp;
        public Timestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }
        public long GetTimestamp()
        {
            return timestamp;
        }
        public void SetTimestamp(long timestamp)
        {
            this.timestamp = timestamp;
        }
    }
}
