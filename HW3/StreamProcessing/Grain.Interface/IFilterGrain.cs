using System.Threading.Tasks;
using StreamProcessing.Function;

namespace StreamProcessing.Grain.Interface
{
    public interface IFilterGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(MyType e);
    }
}
