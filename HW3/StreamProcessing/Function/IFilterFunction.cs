namespace StreamProcessing.Function
{
    public interface IFilterFunction<MyType>
    {
        bool Apply(MyType e);
    }
}
