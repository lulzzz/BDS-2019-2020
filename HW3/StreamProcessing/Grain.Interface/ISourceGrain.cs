using System;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface ISourceGrain : Orleans.IGrainWithGuidKey
    {
        Task Init();
    }
}
