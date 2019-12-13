using System.Threading.Tasks;
using StreamProcessing.Function;

namespace StreamProcessing.Grain.Interface
{
    public interface ISinkGrain : Orleans.IGrainWithIntegerCompoundKey
    {
        Task Process(MyType e);
    }
}