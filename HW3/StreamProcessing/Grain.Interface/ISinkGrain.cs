using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface ISinkGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}