using System;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface ISinkGrain : Orleans.IGrainWithGuidKey
    {
        Task Init();
    }
}
