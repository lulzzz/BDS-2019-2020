using Orleans;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using System;
using System.Collections.Generic;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T>: Grain, IFlatMap, IFlatMapFunction<T>
    {
        public abstract List<T> Apply(T e);
        public Task Process(object e) // Implements the Process method from IFilter
        {
        
            this.GrainFactory.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(Apply((T)e));
            return Task.CompletedTask;
        }
    }

    public class Split : FlatMapGrain<String>
    {
        public override List<String> Apply(String e)
        {
            return e.Split(" ");
        }


    }
}

