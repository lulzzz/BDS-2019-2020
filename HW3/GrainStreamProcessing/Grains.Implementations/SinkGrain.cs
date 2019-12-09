using System;
using System.Threading.Tasks;
using Grains.Interfaces;

namespace Grains.Implementations
{
    public class SinkGrain : Orleans.Grain, ISink
    {
        public Task Process<T>(T e)
        {
            Console.WriteLine(e);
            return Task.CompletedTask;
        }
    }
}