using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IFlatMapGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}