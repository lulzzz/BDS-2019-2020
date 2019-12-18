using System;

namespace StreamProcessing.Function
{
    [Serializable]
    public struct MyType : IComparable<MyType>
    {
        public string key;
        public string value;
        public Timestamp timestamp;

        public MyType(string k, string v, Timestamp t)
        {
            key = k;
            value = v;
            timestamp = t;
        }

        public int CompareTo(MyType x)
        {
            if (x.key == key && x.value == value && x.timestamp.GetTimestamp() == timestamp.GetTimestamp()) return 0;
            return 1;
        }
    }
}
