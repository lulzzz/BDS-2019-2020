using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IJobManagerGrain : Orleans.IGrainWithIntegerKey
    {
        // call by the client
        void registerISourceGrain(Guid opID, String userDefinedFunction, String Key, String Value);
        void registerISinkGrain(Guid opID, String userDefinedFunction, String Key, String Value);
        void registerIJoinGrain(Guid opID, String userDefinedFunction, String Key, String Value);
        void registerIFlatMapGrain(Guid opID, String userDefinedFunction, String Key, String Value);
        void registerIFilterGrain(Guid opID, String userDefinedFunction, String Key, String Value);
        String getKey(Guid opID);
        String getValue(Guid opID);

        void registerSubscribe(Guid opID, Guid streamID);
        void registerPublish(Guid opID, Guid streamID);
        List<Guid> getPublish(Guid grainID);
        List<Guid> getSubscribe(Guid grainID);

        void registerWindow(Guid opID, long window_length, long window_slide);
        void registerAllowedDelay(long delay);
        Tuple<long, long> getWindow(Guid opID);
        long getDelay();
    }
}