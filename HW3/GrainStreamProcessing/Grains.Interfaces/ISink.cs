using System.Threading.Tasks;

namespace Grains.Interfaces
{
    public interface ISink : Orleans.IGrainWithGuidKey
    {
        Task Process<T>(T e);
    }
}