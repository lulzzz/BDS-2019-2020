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
        private Dictionary<Guid, Tuple<Guid, Guid>> two_source_subscribes;
        private Dictionary<Guid, List<Guid>> publishes;
        private Dictionary<Guid, Tuple<long, long>> windows;
        private long AllowedDelay;
        private string NameSpace;

        public override Task OnActivateAsync()
        {
            operators = new Dictionary<Guid, Tuple<string, string, string>>();
            subscribes = new Dictionary<Guid, List<Guid>>();
            two_source_subscribes = new Dictionary<Guid, Tuple<Guid, Guid>>();
            publishes = new Dictionary<Guid, List<Guid>>();
            windows = new Dictionary<Guid, Tuple<long, long>>();
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

        public Task RegisterWindow(Guid opID, long length, long slide)
        {
            if (windows.ContainsKey(opID))
                throw new Exception($"Exception: the window for {opID} is already registered in windows. ");
            windows.Add(opID, new Tuple<long, long>(length, slide));
            return Task.CompletedTask;
        }

        public Task RegisterAllowedDelay(long delay)
        {
            AllowedDelay = delay;
            return Task.CompletedTask;
        }

        public Task<Tuple<long, long>> GetWindow(Guid opID)
        {
            return Task.FromResult(windows[opID]);
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

        public Task RegisterTwoSourceSubscribe(Guid opID, Guid streamID1, Guid streamID2)
        {
            if (two_source_subscribes.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} already subscribes two streams. ");
            two_source_subscribes.Add(opID, new Tuple<Guid, Guid>(streamID1, streamID2));
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

        public Task<Tuple<Guid, Guid>> GetTwoSourceSubscribe(Guid grainID)
        {
            return Task.FromResult(two_source_subscribes[grainID]);
        }

        public async Task RegisterISourceGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<ISourceGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }

        public async Task RegisterISinkGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<ISinkGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }

        public async Task RegisterIJoinGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IJoinGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }

        public async Task RegisterIFlatMapGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IFlatMapGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }

        public async Task RegisterIFilterGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IFilterGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }

        public async Task RegisterIWindowAggregateGrain(Guid opID, string userDefinedFunction, string Key, string Value)
        {
            if (operators.ContainsKey(opID))
                throw new Exception($"Exception: op: {opID} is already registered in operators. ");
            operators.Add(opID, new Tuple<string, string, string>(userDefinedFunction, Key, Value));
            var grain = GrainFactory.GetGrain<IWindowAggregateGrain>(opID, NameSpace + userDefinedFunction);
            await grain.Init();
        }
    }
}
