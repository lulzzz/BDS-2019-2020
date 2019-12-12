namespace StreamProcessing.Function
{
    public interface IFilterFunction<T>
    {
        bool Apply(T e);
    }
}
