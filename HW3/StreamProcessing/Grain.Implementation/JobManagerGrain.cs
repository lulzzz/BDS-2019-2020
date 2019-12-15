using System;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;

namespace Grain.Implementation
{
    public class JobManagerGrain : Orleans.Grain, IJobManagerGrain
    {
        private Dictionary<Guid, Tuple<String, String, String>> operators;
        private Dictionary<Guid, List<Guid>> subscribes;
        private Dictionary<Guid, List<Guid>> publishes;
        private Dictionary<Guid, Tuple<long, long>> windows;
        private long AllowedDelay;
        private String NameSpace;

        public override Task OnActivateAsync()
        {
            operators = null;
            subscribes = null;
            publishes = null;
            windows = null;
            AllowedDelay = 0;
            NameSpace = "StreamProcessing.Grain.Implementation.";
            return Task.CompletedTask;
        }

        public String getKey(Guid opID)
        {
            if (operators.ContainsKey(opID)) return operators[opID].Item2;
            throw new Exception($"Exception: the op: {opID} is not registered in operators. ");
        }
        public String getValue(Guid opID)
        {
            if (operators.ContainsKey(opID)) return operators[opID].Item3;
            throw new Exception($"Exception: the op: {opID} is not registered in operators. ");
        }

        public void registerWindow(Guid opID, long window_length, long window_slide)
        {
            if (windows.ContainsKey(opID))
                throw new Exception($"Exception: the window for op: {opID} is already registered in windows. ");
            windows.Add(opID, new Tuple<long, long>(window_length, window_slide));
        }

        public void registerAllowedDelay(long delay)
        {
            AllowedDelay = delay;
        }

        public Tuple<long, long> getWindow(Guid opID)
        {
            if (windows.ContainsKey(opID)) return windows[opID];
            throw new Exception($"Exception: op: {opID} has't registered any window yet. ");
        }

        public long getDelay()
        {
            return AllowedDelay;
        }

        public void registerPublish(Guid opID, Guid streamID)
        {
            if (publishes.ContainsKey(opID))
            {
                if (publishes[opID].Contains(streamID))
                    throw new Exception($"Exception: op: {opID} stream: {streamID} is already registered in publishes. ");
            }
            else
                publishes.Add(opID, new List<Guid>());

            publishes[opID].Add(streamID);
        }

        public void registerSubscribe(Guid opID, Guid streamID)
        {
            if (subscribes.ContainsKey(opID))
            {
                if (subscribes[opID].Contains(streamID))
                    throw new Exception($"Exception: op: {opID} stream: {streamID} is already registered in subscribes. ");
            }
            else
                subscribes.Add(opID, new List<Guid>());

            subscribes[opID].Add(streamID);
        }

        public List<Guid> getPublish(Guid grainID)
        {
            return publishes[grainID];
        }

        public List<Guid> getSubscribe(Guid grainID)
        {
            return subscribes[grainID];
        }

        public void registerISourceGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<String, String, String>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<ISourceGrain>(opID);
        }

        public void registerISinkGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<String, String, String>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<ISinkGrain>(opID);
        }

        public void registerIJoinGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<String, String, String>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IJoinGrain>(opID, NameSpace + userDefinedFunction);
        }

        public void registerIFlatMapGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<String, String, String>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IFlatMapGrain>(opID, NameSpace + userDefinedFunction);
        }

        public void registerIFilterGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<String, String, String>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IFilterGrain>(opID, NameSpace + userDefinedFunction);
        }
    }
}
