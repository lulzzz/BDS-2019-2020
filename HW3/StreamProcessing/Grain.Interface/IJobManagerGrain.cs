using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Concurrency;

namespace StreamProcessing.Grain.Interface
{
    public interface IJobManagerGrain : Orleans.IGrainWithIntegerCompoundKey
    {
        // reference: https://dotnet.github.io/orleans/Documentation/grains/reentrancy.html
        [AlwaysInterleave]
        Task RegisterISourceGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        [AlwaysInterleave]
        Task RegisterISinkGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        [AlwaysInterleave]
        Task RegisterIJoinGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        [AlwaysInterleave]
        Task RegisterIFlatMapGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        [AlwaysInterleave]
        Task RegisterIFilterGrain(Guid opID, string userDefinedFunction, string Key, string Value);
        [AlwaysInterleave]
        Task RegisterIWindowAggregateGrain(Guid opID, string userDefinedFunction, string Key, string Value);

        [AlwaysInterleave]
        Task<string> GetKey(Guid opID);
        [AlwaysInterleave]
        Task<string> GetValue(Guid opID);

        Task RegisterSubscribe(Guid opID, Guid streamID);
        Task RegisterTwoSourceSubscribe(Guid opID, Guid streamID1, Guid streamID2);
        Task RegisterPublish(Guid opID, Guid streamID);
        [AlwaysInterleave]
        Task<List<Guid>> GetPublish(Guid grainID);
        [AlwaysInterleave]
        Task<List<Guid>> GetSubscribe(Guid grainID);
        [AlwaysInterleave]
        Task<Tuple<Guid, Guid>> GetTwoSourceSubscribe(Guid grainID);

        Task RegisterWindow(Guid opID, long window_length, long window_slide);
        Task RegisterAllowedDelay(long delay);
        [AlwaysInterleave]
        Task<Tuple<long, long>> GetWindow(Guid opID);
        [AlwaysInterleave]
        Task<long> GetDelay();
    }
}