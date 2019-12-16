using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Grain.Implementation
{
    public class JobManagerGrain : Orleans.Grain, IJobManagerGrain
    {
        private Dictionary<Guid, Tuple<string, string, string>> operators;
        private Dictionary<Guid, List<Guid>> subscribes;
        private Dictionary<Guid, List<Guid>> publishes;
        private Dictionary<Guid, Tuple<long, long>> windows;
        private long AllowedDelay;
        private string NameSpace;

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

        public Task<string> GetKey(Guid opID)
        {
            if (operators.ContainsKey(opID)) return Task.FromResult(operators[opID].Item2);
            throw new Exception($"Exception: the op: {opID} is not registered in operators. ");
        }
        public Task<string> GetValue(Guid opID)
        {
            if (operators.ContainsKey(opID)) return Task.FromResult(operators[opID].Item3);
            throw new Exception($"Exception: the op: {opID} is not registered in operators. ");
        }

        public Task RegisterWindow(Guid opID, long window_length, long window_slide)
        {
            if (windows.ContainsKey(opID))
                throw new Exception($"Exception: the window for op: {opID} is already registered in windows. ");
            windows.Add(opID, new Tuple<long, long>(window_length, window_slide));
            return Task.CompletedTask;
        }

        public Task RegisterAllowedDelay(long delay)
        {
            AllowedDelay = delay;
            return Task.CompletedTask;
        }

        public Task<Tuple<long, long>> GetWindow(Guid opID)
        {
            if (windows.ContainsKey(opID)) return Task.FromResult(windows[opID]);
            throw new Exception($"Exception: op: {opID} has't registered any window yet. ");
        }

        public Task<long> GetDelay()
        {
            return Task.FromResult(AllowedDelay);
        }

        public Task RegisterPublish(Guid opID, Guid streamID)
        {
            if (publishes.ContainsKey(opID))
            {
                if (publishes[opID].Contains(streamID))
                    throw new Exception($"Exception: op: {opID} stream: {streamID} is already registered in publishes. ");
            }
            else
                publishes.Add(opID, new List<Guid>());

            publishes[opID].Add(streamID);
            return Task.CompletedTask;
        }

        public Task RegisterSubscribe(Guid opID, Guid streamID)
        {
            if (subscribes.ContainsKey(opID))
            {
                if (subscribes[opID].Contains(streamID))
                    throw new Exception($"Exception: op: {opID} stream: {streamID} is already registered in subscribes. ");
            }
            else
                subscribes.Add(opID, new List<Guid>());

            subscribes[opID].Add(streamID);
            return Task.CompletedTask;
        }

        public Task<List<Guid>> GetPublish(Guid grainID)
        {
            return Task.FromResult(publishes[grainID]);
        }

        public Task<List<Guid>> GetSubscribe(Guid grainID)
        {
            return Task.FromResult(subscribes[grainID]);
        }

        public Task RegisterISourceGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var _ = GrainFactory.GetGrain<ISourceGrain>(opID);
            return Task.CompletedTask;
        }

        public Task RegisterISinkGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var _ = GrainFactory.GetGrain<ISinkGrain>(opID);
            return Task.CompletedTask;
        }

        public Task RegisterIJoinGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var _ = GrainFactory.GetGrain<IJoinGrain>(opID, NameSpace + userDefinedFunction);
            return Task.CompletedTask;
        }

        public Task RegisterIFlatMapGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var _ = GrainFactory.GetGrain<IFlatMapGrain>(opID, NameSpace + userDefinedFunction);
            return Task.CompletedTask;
        }

        public Task RegisterIFilterGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var _ = GrainFactory.GetGrain<IFilterGrain>(opID, NameSpace + userDefinedFunction);
            return Task.CompletedTask;
        }
    }
}
