using System;
using System.Threading.Tasks;
using Functions.Interfaces;

namespace Grains.Interfaces
{
    public interface ISample : Orleans.IGrainWithGuidKey
    {
        Task Init<T>(Guid sinkId, IFilterFunction<T> func);
        Task Process<T>(T e);
    }
}