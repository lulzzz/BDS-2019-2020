using Functions.Interfaces;

namespace Functions.Implementations
{
    public class LargerThanTen : IFilterFunction<long>
    {
        public bool Apply(long e)
        {
            if (e > 10) return true;
            else return false;
        }
    }
}