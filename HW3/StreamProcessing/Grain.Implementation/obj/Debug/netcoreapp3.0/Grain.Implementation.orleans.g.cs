// <auto-generated />
#if !EXCLUDE_CODEGEN
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 618
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
[assembly: global::Orleans.Metadata.FeaturePopulatorAttribute(typeof(OrleansGeneratedCode.OrleansCodeGen6db65d4e47FeaturePopulator))]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute(@"Grain.Implementation, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace OrleansGeneratedCodeD8AB684D
{
    using global::Orleans;
    using global::System.Reflection;
}

namespace OrleansGeneratedCode
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute(@"Orleans-CodeGenerator", @"2.0.0.0")]
    internal sealed class OrleansCodeGen6db65d4e47FeaturePopulator : global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainInterfaceFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainClassFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Serialization.SerializerFeature>
    {
        public void Populate(global::Orleans.Metadata.GrainInterfaceFeature feature)
        {
        }

        public void Populate(global::Orleans.Metadata.GrainClassFeature feature)
        {
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.SplitValue)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.LargerThanTenFilter)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.OddNumberFilter)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.DistanceFilter)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.JobManagerGrain)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.SinkGrain)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.SourceGrain)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.WindowDuplicateRemover)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.WindowCountByKey)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::StreamProcessing.Grain.Implementation.WindowJoinGrain)));
        }

        public void Populate(global::Orleans.Serialization.SerializerFeature feature)
        {
            feature.AddKnownType(@"Interop,System.Runtime.Extensions", @"Interop");
            feature.AddKnownType(@"Interop+Sys,System.Runtime.Extensions", @"Sys");
            feature.AddKnownType(@"FxResources.System.Runtime.Extensions.SR,System.Runtime.Extensions", @"FxResources.System.Runtime.Extensions.SR");
            feature.AddKnownType(@"System.AppDomainUnloadedException,System.Runtime.Extensions", @"AppDomainUnloadedException");
            feature.AddKnownType(@"System.ApplicationId,System.Runtime.Extensions", @"ApplicationId");
            feature.AddKnownType(@"System.ContextBoundObject,System.Runtime.Extensions", @"ContextBoundObject");
            feature.AddKnownType(@"System.ContextMarshalException,System.Runtime.Extensions", @"ContextMarshalException");
            feature.AddKnownType(@"System.ContextStaticAttribute,System.Runtime.Extensions", @"ContextStaticAttribute");
            feature.AddKnownType(@"System.LoaderOptimization,System.Runtime.Extensions", @"LoaderOptimization");
            feature.AddKnownType(@"System.LoaderOptimizationAttribute,System.Runtime.Extensions", @"LoaderOptimizationAttribute");
            feature.AddKnownType(@"System.StringNormalizationExtensions,System.Runtime.Extensions", @"StringNormalizationExtensions");
            feature.AddKnownType(@"System.SR,System.Runtime.Extensions", @"SR");
            feature.AddKnownType(@"System.Text.ValueStringBuilder,System.Runtime.Extensions", @"ValueStringBuilder");
            feature.AddKnownType(@"System.Threading.Tasks.TaskToApm,System.Runtime.Extensions", @"TaskToApm");
            feature.AddKnownType(@"System.Security.Permissions.CodeAccessSecurityAttribute,System.Runtime.Extensions", @"CodeAccessSecurityAttribute");
            feature.AddKnownType(@"System.Security.Permissions.SecurityAttribute,System.Runtime.Extensions", @"SecurityAttribute");
            feature.AddKnownType(@"System.Security.Permissions.SecurityAction,System.Runtime.Extensions", @"SecurityAction");
            feature.AddKnownType(@"System.Security.Permissions.SecurityPermissionAttribute,System.Runtime.Extensions", @"SecurityPermissionAttribute");
            feature.AddKnownType(@"System.Security.Permissions.SecurityPermissionFlag,System.Runtime.Extensions", @"SecurityPermissionFlag");
            feature.AddKnownType(@"System.Runtime.ProfileOptimization,System.Runtime.Extensions", @"ProfileOptimization");
            feature.AddKnownType(@"System.Runtime.Versioning.FrameworkName,System.Runtime.Extensions", @"FrameworkName");
            feature.AddKnownType(@"System.Runtime.Versioning.ComponentGuaranteesAttribute,System.Runtime.Extensions", @"ComponentGuaranteesAttribute");
            feature.AddKnownType(@"System.Runtime.Versioning.ResourceConsumptionAttribute,System.Runtime.Extensions", @"ResourceConsumptionAttribute");
            feature.AddKnownType(@"System.Runtime.Versioning.ComponentGuaranteesOptions,System.Runtime.Extensions", @"ComponentGuaranteesOptions");
            feature.AddKnownType(@"System.Runtime.Versioning.ResourceExposureAttribute,System.Runtime.Extensions", @"ResourceExposureAttribute");
            feature.AddKnownType(@"System.Runtime.Versioning.ResourceScope,System.Runtime.Extensions", @"ResourceScope");
            feature.AddKnownType(@"System.Runtime.Versioning.SxSRequirements,System.Runtime.Extensions", @"SxSRequirements");
            feature.AddKnownType(@"System.Runtime.Versioning.VersioningHelper,System.Runtime.Extensions", @"VersioningHelper");
            feature.AddKnownType(@"System.Runtime.CompilerServices.SwitchExpressionException,System.Runtime.Extensions", @"SwitchExpressionException");
            feature.AddKnownType(@"System.Reflection.AssemblyNameProxy,System.Runtime.Extensions", @"AssemblyNameProxy");
            feature.AddKnownType(@"System.Net.WebUtility,System.Runtime.Extensions", @"WebUtility");
            feature.AddKnownType(@"System.IO.StringReader,System.Runtime.Extensions", @"StringReader");
            feature.AddKnownType(@"System.IO.StringWriter,System.Runtime.Extensions", @"StringWriter");
            feature.AddKnownType(@"System.IO.BufferedStream,System.Runtime.Extensions", @"BufferedStream");
            feature.AddKnownType(@"System.IO.InvalidDataException,System.Runtime.Extensions", @"InvalidDataException");
            feature.AddKnownType(@"System.IO.StreamHelpers,System.Runtime.Extensions", @"StreamHelpers");
            feature.AddKnownType(@"System.Diagnostics.Stopwatch,System.Runtime.Extensions", @"Stopwatch");
            feature.AddKnownType(@"System.CodeDom.Compiler.IndentedTextWriter,System.Runtime.Extensions", @"IndentedTextWriter");
            feature.AddKnownType(@"Interop,System.Console", @"Interop");
            feature.AddKnownType(@"Interop+Error,System.Console", @"Error");
            feature.AddKnownType(@"Interop+ErrorInfo,System.Console", @"ErrorInfo");
            feature.AddKnownType(@"Interop+Sys,System.Console", @"Sys");
            feature.AddKnownType(@"Interop+Sys+FileDescriptors,System.Console", @"FileDescriptors");
            feature.AddKnownType(@"Interop+Sys+ControlCharacterNames,System.Console", @"ControlCharacterNames");
            feature.AddKnownType(@"Interop+Sys+SeekWhence,System.Console", @"SeekWhence");
            feature.AddKnownType(@"Interop+Sys+OpenFlags,System.Console", @"OpenFlags");
            feature.AddKnownType(@"Interop+Sys+PollEvents,System.Console", @"PollEvents");
            feature.AddKnownType(@"Interop+Sys+PollEvent,System.Console", @"PollEvent");
            feature.AddKnownType(@"Interop+Sys+Passwd,System.Console", @"Passwd");
            feature.AddKnownType(@"Interop+Sys+CtrlCode,System.Console", @"CtrlCode");
            feature.AddKnownType(@"Interop+Sys+CtrlCallback,System.Console", @"CtrlCallback");
            feature.AddKnownType(@"Interop+Sys+TerminalInvalidationCallback,System.Console", @"TerminalInvalidationCallback");
            feature.AddKnownType(@"Interop+Sys+FileStatus,System.Console", @"FileStatus");
            feature.AddKnownType(@"Interop+Sys+FileStatusFlags,System.Console", @"FileStatusFlags");
            feature.AddKnownType(@"Interop+Sys+WinSize,System.Console", @"WinSize");
            feature.AddKnownType(@"FxResources.System.Console.SR,System.Console", @"FxResources.System.Console.SR");
            feature.AddKnownType(@"Microsoft.Win32.SafeHandles.SafeFileHandleHelper,System.Console", @"Microsoft.Win32.SafeHandles.SafeFileHandleHelper");
            feature.AddKnownType(@"System.Console,System.Console", @"Console");
            feature.AddKnownType(@"System.ConsoleCancelEventHandler,System.Console", @"ConsoleCancelEventHandler");
            feature.AddKnownType(@"System.ConsoleCancelEventArgs,System.Console", @"ConsoleCancelEventArgs");
            feature.AddKnownType(@"System.ConsoleColor,System.Console", @"ConsoleColor");
            feature.AddKnownType(@"System.ConsoleSpecialKey,System.Console", @"ConsoleSpecialKey");
            feature.AddKnownType(@"System.ConsoleKey,System.Console", @"ConsoleKey");
            feature.AddKnownType(@"System.ConsoleKeyInfo,System.Console", @"ConsoleKeyInfo");
            feature.AddKnownType(@"System.ConsoleModifiers,System.Console", @"ConsoleModifiers");
            feature.AddKnownType(@"System.ConsolePal,System.Console", @"ConsolePal");
            feature.AddKnownType(@"System.ConsolePal+TerminalFormatStrings,System.Console", @"TerminalFormatStrings");
            feature.AddKnownType(@"System.IO.ConsoleStream,System.Console", @"ConsoleStream");
            feature.AddKnownType(@"System.ConsolePal+ControlCHandlerRegistrar,System.Console", @"ControlCHandlerRegistrar");
            feature.AddKnownType(@"System.TermInfo,System.Console", @"TermInfo");
            feature.AddKnownType(@"System.TermInfo+WellKnownNumbers,System.Console", @"WellKnownNumbers");
            feature.AddKnownType(@"System.TermInfo+WellKnownStrings,System.Console", @"WellKnownStrings");
            feature.AddKnownType(@"System.TermInfo+Database,System.Console", @"Database");
            feature.AddKnownType(@"System.TermInfo+ParameterizedStrings,System.Console", @"ParameterizedStrings");
            feature.AddKnownType(@"System.TermInfo+ParameterizedStrings+FormatParam,System.Console", @"ParameterizedStrings.FormatParam");
            feature.AddKnownType(@"System.SR,System.Console", @"SR");
            feature.AddKnownType(@"System.Text.EncodingExtensions,System.Console", @"EncodingExtensions");
            feature.AddKnownType(@"System.Text.ConsoleEncoding,System.Console", @"ConsoleEncoding");
            feature.AddKnownType(@"System.Text.StringBuilderCache,System.Console", @"StringBuilderCache");
            feature.AddKnownType(@"System.Text.EncodingHelper,System.Console", @"EncodingHelper");
            feature.AddKnownType(@"System.Text.StringOrCharArray,System.Console", @"StringOrCharArray");
            feature.AddKnownType(@"System.IO.SyncTextReader,System.Console", @"SyncTextReader");
            feature.AddKnownType(@"System.IO.Error,System.Console", @"Error");
            feature.AddKnownType(@"System.IO.StdInReader,System.Console", @"StdInReader");
            feature.AddKnownType(@"System.IO.PersistedFiles,System.Console", @"PersistedFiles");
            feature.AddKnownType(@"FxResources.System.Linq.SR,System.Linq", @"FxResources.System.Linq.SR");
            feature.AddKnownType(@"System.SR,System.Linq", @"SR");
            feature.AddKnownType(@"System.Collections.Generic.LargeArrayBuilder`1,System.Linq", @"LargeArrayBuilder`1'1");
            feature.AddKnownType(@"System.Collections.Generic.ArrayBuilder`1,System.Linq", @"ArrayBuilder`1'1");
            feature.AddKnownType(@"System.Collections.Generic.EnumerableHelpers,System.Linq", @"EnumerableHelpers");
            feature.AddKnownType(@"System.Collections.Generic.CopyPosition,System.Linq", @"CopyPosition");
            feature.AddKnownType(@"System.Collections.Generic.Marker,System.Linq", @"Marker");
            feature.AddKnownType(@"System.Collections.Generic.SparseArrayBuilder`1,System.Linq", @"SparseArrayBuilder`1'1");
            feature.AddKnownType(@"System.Linq.Enumerable,System.Linq", @"Enumerable");
            feature.AddKnownType(@"System.Linq.Enumerable+Iterator`1,System.Linq", @"Iterator`1'1");
            feature.AddKnownType(@"System.Linq.IIListProvider`1,System.Linq", @"IIListProvider`1'1");
            feature.AddKnownType(@"System.Linq.IPartition`1,System.Linq", @"IPartition`1'1");
            feature.AddKnownType(@"System.Linq.Enumerable+WhereArrayIterator`1,System.Linq", @"WhereArrayIterator`1'1");
            feature.AddKnownType(@"System.Linq.GroupedResultEnumerable`4,System.Linq", @"GroupedResultEnumerable`4'4");
            feature.AddKnownType(@"System.Linq.GroupedResultEnumerable`3,System.Linq", @"GroupedResultEnumerable`3'3");
            feature.AddKnownType(@"System.Linq.GroupedEnumerable`3,System.Linq", @"GroupedEnumerable`3'3");
            feature.AddKnownType(@"System.Linq.IGrouping`2,System.Linq", @"IGrouping`2'2");
            feature.AddKnownType(@"System.Linq.GroupedEnumerable`2,System.Linq", @"GroupedEnumerable`2'2");
            feature.AddKnownType(@"System.Linq.Lookup`2,System.Linq", @"Lookup`2'2");
            feature.AddKnownType(@"System.Linq.ILookup`2,System.Linq", @"ILookup`2'2");
            feature.AddKnownType(@"System.Linq.OrderedEnumerable`1,System.Linq", @"OrderedEnumerable`1'1");
            feature.AddKnownType(@"System.Linq.IOrderedEnumerable`1,System.Linq", @"IOrderedEnumerable`1'1");
            feature.AddKnownType(@"System.Linq.EmptyPartition`1,System.Linq", @"EmptyPartition`1'1");
            feature.AddKnownType(@"System.Linq.OrderedPartition`1,System.Linq", @"OrderedPartition`1'1");
            feature.AddKnownType(@"System.Linq.Buffer`1,System.Linq", @"Buffer`1'1");
            feature.AddKnownType(@"System.Linq.SystemCore_EnumerableDebugView`1,System.Linq", @"SystemCore_EnumerableDebugView`1'1");
            feature.AddKnownType(@"System.Linq.SystemCore_EnumerableDebugViewEmptyException,System.Linq", @"SystemCore_EnumerableDebugViewEmptyException");
            feature.AddKnownType(@"System.Linq.SystemCore_EnumerableDebugView,System.Linq", @"SystemCore_EnumerableDebugView");
            feature.AddKnownType(@"System.Linq.SystemLinq_GroupingDebugView`2,System.Linq", @"SystemLinq_GroupingDebugView`2'2");
            feature.AddKnownType(@"System.Linq.SystemLinq_LookupDebugView`2,System.Linq", @"SystemLinq_LookupDebugView`2'2");
            feature.AddKnownType(@"System.Linq.Grouping`2,System.Linq", @"Grouping`2'2");
            feature.AddKnownType(@"System.Linq.OrderedEnumerable`2,System.Linq", @"OrderedEnumerable`2'2");
            feature.AddKnownType(@"System.Linq.CachingComparer`1,System.Linq", @"CachingComparer`1'1");
            feature.AddKnownType(@"System.Linq.CachingComparer`2,System.Linq", @"CachingComparer`2'2");
            feature.AddKnownType(@"System.Linq.CachingComparerWithChild`2,System.Linq", @"CachingComparerWithChild`2'2");
            feature.AddKnownType(@"System.Linq.EnumerableSorter`1,System.Linq", @"EnumerableSorter`1'1");
            feature.AddKnownType(@"System.Linq.EnumerableSorter`2,System.Linq", @"EnumerableSorter`2'2");
            feature.AddKnownType(@"System.Linq.Set`1,System.Linq", @"Set`1'1");
            feature.AddKnownType(@"System.Linq.SingleLinkedNode`1,System.Linq", @"SingleLinkedNode`1'1");
            feature.AddKnownType(@"System.Linq.ThrowHelper,System.Linq", @"ThrowHelper");
            feature.AddKnownType(@"System.Linq.ExceptionArgument,System.Linq", @"ExceptionArgument");
            feature.AddKnownType(@"System.Linq.Utilities,System.Linq", @"Utilities");
            feature.AddKnownType(@"GrainStreamProcessing.GrainImpl.FlatMapGrain,Grain.Implementation", @"GrainStreamProcessing.GrainImpl.FlatMapGrain");
            feature.AddKnownType(@"StreamProcessing.Function.IFlatMapFunction,Function", @"StreamProcessing.Function.IFlatMapFunction");
            feature.AddKnownType(@"GrainStreamProcessing.GrainImpl.SplitValue,Grain.Implementation", @"GrainStreamProcessing.GrainImpl.SplitValue");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.FilterGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.FilterGrain");
            feature.AddKnownType(@"StreamProcessing.Function.IFilterFunction,Function", @"StreamProcessing.Function.IFilterFunction");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.LargerThanTenFilter,Grain.Implementation", @"StreamProcessing.Grain.Implementation.LargerThanTenFilter");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.OddNumberFilter,Grain.Implementation", @"StreamProcessing.Grain.Implementation.OddNumberFilter");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.DistanceFilter,Grain.Implementation", @"StreamProcessing.Grain.Implementation.DistanceFilter");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.JobManagerGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.JobManagerGrain");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.SinkGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.SinkGrain");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.SourceGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.SourceGrain");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.WindowAggregateGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.WindowAggregateGrain");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.WindowDuplicateRemover,Grain.Implementation", @"StreamProcessing.Grain.Implementation.WindowDuplicateRemover");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.WindowCountByKey,Grain.Implementation", @"StreamProcessing.Grain.Implementation.WindowCountByKey");
            feature.AddKnownType(@"StreamProcessing.Grain.Implementation.WindowJoinGrain,Grain.Implementation", @"StreamProcessing.Grain.Implementation.WindowJoinGrain");
            feature.AddKnownType(@"StreamProcessing.Function.MyType,Function", @"StreamProcessing.Function.MyType");
            feature.AddKnownType(@"StreamProcessing.Function.NewEvent,Function", @"StreamProcessing.Function.NewEvent");
            feature.AddKnownType(@"StreamProcessing.Function.Timestamp,Function", @"StreamProcessing.Function.Timestamp");
            feature.AddKnownType(@"StreamProcessing.Function.WindowFunction,Function", @"StreamProcessing.Function.WindowFunction");
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
