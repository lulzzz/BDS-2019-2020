using System;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Grain.Implementation
{
    public abstract class FilterGrain<MyType> : Orleans.Grain, IFilterGrain, IFilterFunction<MyType>
    {
        public abstract bool Apply(MyType e);
        public Task Process(object e) // Implements the Process method from IFilter
        {
            if (Apply((MyType)e)) // If the function returns true, send the element to SinkGrain
            {
                this.GrainFactory.GetGrain<ISinkGrain>(0, "StreamProcessing.Grain.Implementation.SinkGrain").Process(e);
            } // Otherwise, skip it
            return Task.CompletedTask;
        }
    }
    
    public class LargerThanTenFilter : FilterGrain<MyType>
    {
        public override bool Apply(MyType e) // Implements the Apply method, filtering numbers larger than 10
        {
            int value = Convert.ToInt32(e.value);
            if (value > 10) return true;
            else return false;
        }
    }

    public class OddNumberFilter : FilterGrain<MyType>
    {
        public override bool Apply(MyType e) // Implements the Apply method, filtering odd numbers
        {
            int value = Convert.ToInt32(e.value);
            if (value % 2 == 1) return true;
            else return false;
        }
    }
}
