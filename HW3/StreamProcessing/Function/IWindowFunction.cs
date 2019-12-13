using System;
namespace Function
{
    public interface IWindowFunction<T>
    {

    }
}


using System.Collections.Generic;
namespace StreamProcessing.Function
{
    public interface IFlatMapFunction<T>
    {
        List<T> Apply(T e);
    }
}
