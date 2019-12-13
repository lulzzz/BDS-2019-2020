using System.Threading.Tasks;
using StreamProcessing.Function;

namespace StreamProcessing.Grain.Interface
{
    public interface IJoinGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(MyType e1, MyType e2);
    }
}
