﻿using System;
using System.Threading.Tasks;
using StreamProcessing.Function;
using StreamProcessing.Grain.Interface;


namespace StreamProcessing.Grain.Implementation
{
    public class SinkGrain : Orleans.Grain, ISinkGrain
    {
        public Task Process(MyType e)
        {
            Console.WriteLine(e);// Output received data to Silo console
            return Task.CompletedTask;
        }
    }
}