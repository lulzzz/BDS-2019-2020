using System;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;

namespace GrainStreamProcessing.GrainInterfaces
{
    public interface IFlatMap : Orleans.IGrainWithIntegerKey
    {
        Task Process(object e);
    }
}
