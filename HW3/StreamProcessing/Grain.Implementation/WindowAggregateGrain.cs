using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Grain.Implementation
{
    public class WindowAggregateGrain : Orleans.Grain, IWindowAggregateGrain
    {
        public Task Init()
        {
            return Task.CompletedTask;
        }


    }
}
