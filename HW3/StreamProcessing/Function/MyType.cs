namespace StreamProcessing.Function
{
    public struct MyType
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
    }
}
