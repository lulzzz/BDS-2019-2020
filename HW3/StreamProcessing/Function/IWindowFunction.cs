using System;
using System.Collections.Generic;

namespace StreamProcessing.Function
{
    public interface IWindowFunction<T>
    {
        List<T> Apply(T e);
        List<T> Trigger(int watermark);
    }
}