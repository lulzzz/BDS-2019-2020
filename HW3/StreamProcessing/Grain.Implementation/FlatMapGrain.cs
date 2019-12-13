using Orleans;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;
using System;
using System.Collections.Generic;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain : Grain, IFlatMapGrain, IFlatMapFunction
    {
        public abstract List<MyType> Apply(MyType e);

        public Task Process(object e) // Implements the Process method from IFilter
        {
            List<MyType> result = Apply((MyType)e);
            foreach (MyType r in result)
            {
                this.GrainFactory.GetGrain<ISinkGrain>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(r);
            }
            
            return Task.CompletedTask;
        }
    }

    public class Split : FlatMapGrain
    {
        public override List<MyType> Apply(MyType e)
        {
            String key = e.key;
            String value = e.value;
            Timestamp time = e.timestamp;


            List<MyType> strs = new List<MyType>();
            String[] parts = value.Split(" ");
            foreach (String part in parts)
            {
                MyType new_part = new MyType(key, part, time);
                strs.Add(new_part);
            }
            return strs;
        }
    }
}
