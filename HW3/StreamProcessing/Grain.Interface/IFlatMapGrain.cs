﻿using System;
using System.Threading.Tasks;

namespace StreamProcessing.Grain.Interface
{
    public interface IFlatMapGrain : Orleans.IGrainWithGuidKey
    {
        Task Init();
    }
}
