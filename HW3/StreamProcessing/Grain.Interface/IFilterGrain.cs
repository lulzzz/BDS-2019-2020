using System;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IFilterGrain : Orleans.IGrainWithGuidKey
    {
        Task Init();
    }
}
