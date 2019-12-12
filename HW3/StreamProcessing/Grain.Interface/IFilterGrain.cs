using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IFilterGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}
