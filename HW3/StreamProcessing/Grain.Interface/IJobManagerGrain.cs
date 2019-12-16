using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IJobManagerGrain : Orleans.IGrainWithIntegerCompoundKey
    {
        Task RegisterISourceGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        Task RegisterISinkGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        Task RegisterIJoinGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        Task RegisterIFlatMapGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        Task RegisterIFilterGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        Task<string> GetKey(Guid opID);
        Task<string> GetValue(Guid opID);

        Task RegisterSubscribe(Guid opID, Guid streamID);
        Task RegisterPublish(Guid opID, Guid streamID);
        Task<List<Guid>> GetPublish(Guid grainID);
        Task<List<Guid>> GetSubscribe(Guid grainID);

        Task RegisterWindow(Guid opID, long window_length, long window_slide);
        Task RegisterAllowedDelay(long delay);
        Task<Tuple<long, long>> GetWindow(Guid opID);
        Task<long> GetDelay();
    }
}