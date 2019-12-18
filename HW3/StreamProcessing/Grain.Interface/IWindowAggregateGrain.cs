using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IWindowAggregateGrain : Orleans.IGrainWithGuidKey
    {
        Task Init();
    }
}
