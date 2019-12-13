using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface ISinkGrain : Orleans.IGrainWithIntegerCompoundKey
    {
        Task Process(object e);
    }
}