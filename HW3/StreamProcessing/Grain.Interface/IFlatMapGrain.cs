using System.Threading.Tasks;
using StreamProcessing.Function;

namespace StreamProcessing.Grain.Interface
{
    public interface IFlatMapGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(MyType e);
    }
}