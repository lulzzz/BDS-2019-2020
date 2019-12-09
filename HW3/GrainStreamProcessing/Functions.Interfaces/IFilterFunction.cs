namespace Functions.Interfaces
{
    public interface IFilterFunction<T>
    {
        bool Apply(T e);
    }
}