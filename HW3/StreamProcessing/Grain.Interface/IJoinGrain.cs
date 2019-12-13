using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IJoinGrain : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e1, object e2);
    }
}
