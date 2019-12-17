using System;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IJoinGrain : Orleans.IGrainWithGuidKey
    { 
        Task Init();
    }
}
