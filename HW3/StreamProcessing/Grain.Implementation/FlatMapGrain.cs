using Orleans;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;
using System;
using System.Collections.Generic;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T> : Grain, IFlatMapGrain, IFlatMapFunction<T>
    {
        public abstract List<T> Apply(T e);
        public Task Process(object e) // Implements the Process method from IFilter
        {
            List<T> result = Apply((T)e);
            this.GrainFactory.GetGrain<ISinkGrain>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(result);
            return Task.CompletedTask;
        }
    }

    public class Split : FlatMapGrain<String>
    {
        public override List<String> Apply(String e)
        {
            List<String> strs = new List<String>();
            String[] parts = e.Split(" ");
            foreach (String part in parts)
            {
                strs.Add(part);
            }
            return strs;
        }
    }
}