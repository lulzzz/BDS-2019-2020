using System.Collections.Generic;
namespace StreamProcessing.Function
{
    public interface IFlatMapFunction
    {
        List<MyType> Apply(MyType e);
    }
}
