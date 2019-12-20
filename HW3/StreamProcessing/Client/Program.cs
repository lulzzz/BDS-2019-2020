using System;
using Orleans;
using Orleans.Hosting;
using Orleans.Configuration;
using System.Threading.Tasks;
using StreamProcessing.Function;
using Microsoft.Extensions.Logging;
using StreamProcessing.Grain.Interface;

namespace StreamProcessing.Client
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    //await Client1(client);
                    //await Client2(client);
                    //await Client3(client);
                    await TestClient(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.StackTrace}, {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "GrainStreamProcessing";
                })
                //.ConfigureApplicationParts(parts => parts
                //.AddApplicationPart(typeof(IJobManagerGrain).Assembly).WithReferences()
                //)
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("SMSProvider")
                .Build();

            int i = 0;
            int maxReTry = 10;
            while (i < maxReTry)
            {
                try
                {
                    await client.Connect();
                    i = maxReTry;
                }
                catch (Exception e)
                {
                    if (i < maxReTry - 1) i++;
                    else throw new Exception($"Exception: {e.Message}");
                }
            }
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task TestClient(IClusterClient client)
        {
            // STEP 1: create IDs for all grains and streams
            var tagSourceGrain = Guid.NewGuid();
            var gpsSourceGrain = Guid.NewGuid();
            var photoSourceGrain = Guid.NewGuid();
            var joinTagPhotoGrain = Guid.NewGuid();
            var joinGPSGPSGrain = Guid.NewGuid();
            var joinBothGrain = Guid.NewGuid();
            var filterGrain = Guid.NewGuid();
            var windowDuplicateRemoveGrain = Guid.NewGuid();
            var windowAggregateGrain = Guid.NewGuid();
            var sinkGrain = Guid.NewGuid();

            var SourceTagStream = Guid.NewGuid();
            var SourceGPSStream = Guid.NewGuid();
            var SourcePhotoStream = Guid.NewGuid();
            var TagStream  = Guid.NewGuid();
            var PhotoStream = Guid.NewGuid();
            var GPS1Stream = Guid.NewGuid();
            var GPS2Stream = Guid.NewGuid();
            var JoinedTagPhotoStream = Guid.NewGuid();
            var JoinedGPSGPSStream = Guid.NewGuid();
            var JoinedBothStream = Guid.NewGuid();
            var FilteredStream = Guid.NewGuid();
            var DistinctStream = Guid.NewGuid();
            var AggregatedStream = Guid.NewGuid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register window informations
            long window_length = 15000;
            long window_slide = 5000;
            long delay = 0;
            await jobManager.RegisterWindow(joinTagPhotoGrain,window_length, window_slide);
            await jobManager.RegisterWindow(joinGPSGPSGrain, window_length, window_slide);
            await jobManager.RegisterWindow(joinBothGrain, window_slide, window_slide);
            await jobManager.RegisterWindow(windowDuplicateRemoveGrain, window_slide, window_slide);
            await jobManager.RegisterWindow(windowAggregateGrain, window_slide, window_slide);
            await jobManager.RegisterAllowedDelay(delay);

            // STEP 3: register all subscribes and publishes, which only add the information to JobManager
            await jobManager.RegisterSubscribe(tagSourceGrain, SourceTagStream);
            await jobManager.RegisterSubscribe(gpsSourceGrain, SourceGPSStream);
            await jobManager.RegisterSubscribe(photoSourceGrain, SourcePhotoStream);
            await jobManager.RegisterTwoSourceSubscribe(joinTagPhotoGrain, TagStream, PhotoStream);
            await jobManager.RegisterTwoSourceSubscribe(joinGPSGPSGrain, GPS1Stream, GPS2Stream);
            await jobManager.RegisterTwoSourceSubscribe(joinBothGrain, JoinedTagPhotoStream, JoinedGPSGPSStream);
            await jobManager.RegisterSubscribe(filterGrain, JoinedBothStream);
            await jobManager.RegisterSubscribe(windowDuplicateRemoveGrain, FilteredStream);
            await jobManager.RegisterSubscribe(windowAggregateGrain, DistinctStream);
            await jobManager.RegisterSubscribe(sinkGrain, AggregatedStream);

            await jobManager.RegisterPublish(tagSourceGrain, TagStream);
            await jobManager.RegisterPublish(photoSourceGrain, PhotoStream);
            await jobManager.RegisterPublish(gpsSourceGrain, GPS1Stream);
            await jobManager.RegisterPublish(gpsSourceGrain, GPS2Stream);
            await jobManager.RegisterPublish(joinTagPhotoGrain, JoinedTagPhotoStream);
            await jobManager.RegisterPublish(joinGPSGPSGrain, JoinedGPSGPSStream);
            await jobManager.RegisterPublish(joinBothGrain, JoinedBothStream);
            await jobManager.RegisterPublish(filterGrain, FilteredStream);
            await jobManager.RegisterPublish(windowDuplicateRemoveGrain, DistinctStream);
            await jobManager.RegisterPublish(windowAggregateGrain, AggregatedStream);

            // STEP 4: register the operators, which will activate the grain
            await jobManager.RegisterISourceGrain(tagSourceGrain, "SourceGrain", "", "0 1");                                      // tag = <photo_id, user_id>
            await jobManager.RegisterISourceGrain(gpsSourceGrain, "SourceGrain", "", "0 1 2");                                    // gps = <user_id, lat, lon>
            await jobManager.RegisterISourceGrain(photoSourceGrain, "SourceGrain", "", "0 1 2 3");                                // photo = <photo_id, user_id, lat, lon>
            // For this join operation, tag will be the 1st source, photo will be the 2nd source
            await jobManager.RegisterIJoinGrain(joinTagPhotoGrain, "WindowJoinGrain", "0, 0", "1, 2 3");                          // JoinedTagPhotoStream = <photo_id, user_id, lat1, lon1>
            await jobManager.RegisterIJoinGrain(joinGPSGPSGrain, "WindowJoinGrain", "0 1 2, 0 1 2", ",");                          // JoinedGPSGPSStream = <user_id, lat2, lon2>
            // For this join operation, JoinedTagPhotoStream will be the 1st source, JoinedGPSGPSStream will be the 2nd source
            await jobManager.RegisterIJoinGrain(joinBothGrain, "WindowJoinGrain", "1, 0", "0 2 3, 1 2");                          // JoinedBothStream = <user_id, photo_id, lat1, lon1, lat2, lon2>
            await jobManager.RegisterIFilterGrain(filterGrain, "DistanceFilter", "0 1", "2 3 4 5");                               // FilteredStream = <user_id, photo_id, lat1, lon1, lat2, lon2>
     /*       await jobManager.RegisterIWindowAggregateGrain(windowDuplicateRemoveGrain, "WindowDuplicateRemover", "0 1", "");      // DistinctStream = <user_id, photo_id>
            await jobManager.RegisterIWindowAggregateGrain(windowAggregateGrain, "WindowCountByKey", "0", "1");                   // AggregatedStream = <user_id, count>
            await jobManager.RegisterISinkGrain(sinkGrain, "SinkGrain", "", "");*/

            // STEP 5: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var tag = streamProvider.GetStream<string>(SourceTagStream, null);
            var photo = streamProvider.GetStream<string>(SourcePhotoStream, null);
            var gps = streamProvider.GetStream<string>(SourceGPSStream, null);

            // STEP 6: let DataDriver feeds data to streams
            await DataDriver.Run(photo, tag, gps, 1600, 0);
        }

        private static async Task Client3(IClusterClient client)
        {
            // STEP 1: create IDs for all grains and streams
            var tagSourceGrain = Guid.NewGuid();
            var photoSourceGrain = Guid.NewGuid();
            var tagFilterGrain = Guid.NewGuid();
            var windowJoinGrain = Guid.NewGuid();
            var sinkGrain = Guid.NewGuid();

            var SourceTagStream = Guid.NewGuid();
            var SourcePhotoStream = Guid.NewGuid();
            var TagStream = Guid.NewGuid();
            var PhotoStream = Guid.NewGuid();
            var FilteredTagStream = Guid.NewGuid();
            var JoinedStream = Guid.NewGuid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register window informations
            long window_length = 15000;
            long window_slide = 5000;
            long delay = 0;
            await jobManager.RegisterWindow(windowJoinGrain, window_length, window_slide);
            await jobManager.RegisterAllowedDelay(delay);

            // STEP 3: register all subscribes and publishes, which only add the information to JobManager 
            await jobManager.RegisterSubscribe(tagSourceGrain, SourceTagStream);
            await jobManager.RegisterSubscribe(photoSourceGrain, SourcePhotoStream);
            await jobManager.RegisterSubscribe(tagFilterGrain, TagStream);
            await jobManager.RegisterTwoSourceSubscribe(windowJoinGrain, FilteredTagStream, PhotoStream);
            await jobManager.RegisterSubscribe(sinkGrain, JoinedStream);

            await jobManager.RegisterPublish(tagSourceGrain, TagStream);
            await jobManager.RegisterPublish(photoSourceGrain, PhotoStream);
            await jobManager.RegisterPublish(tagFilterGrain, FilteredTagStream);
            await jobManager.RegisterPublish(windowJoinGrain, JoinedStream);

            // STEP 4: register the operators, which will activate the grain
            // string 1: user defined function
            // string 2 & 3: when the operator receives an event, which columns should be put to new Key and Value
            await jobManager.RegisterISourceGrain(tagSourceGrain, "SourceGrain", "", "0 1");
            await jobManager.RegisterISourceGrain(photoSourceGrain, "SourceGrain", "", "0 1 2 3");
            await jobManager.RegisterIFilterGrain(tagFilterGrain, "LargerThanTenFilter", "0", "1");
            // for Join Grain, string 2 defines which column is the key that user wants to join on
            // string 3 defines which columns that user wants to keep in the result after join operation
            await jobManager.RegisterIJoinGrain(windowJoinGrain, "WindowJoinGrain", "0, 0", "1, 2 3");
            await jobManager.RegisterISinkGrain(sinkGrain, "SinkGrain", "", "");

            // STEP 5: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var tag = streamProvider.GetStream<string>(SourceTagStream, null);
            var photo = streamProvider.GetStream<string>(SourcePhotoStream, null);
            var gps = streamProvider.GetStream<string>(new Guid(), null);

            // STEP 6: let DataDriver feeds data to streams
            await DataDriver.Run(photo, tag, gps, 1600, 0);
        }

        private static async Task Client2(IClusterClient client)
        {
            // STEP 1: create IDs for all grains and streams
            var tagSourceGrain = Guid.NewGuid();
            var photoSourceGrain = Guid.NewGuid();
            var gpsSourceGrain = Guid.NewGuid();
            var sinkGrain = Guid.NewGuid();

            var tag = Guid.NewGuid();
            var photo = Guid.NewGuid();
            var gps = Guid.NewGuid();
            var tagStream = Guid.NewGuid();
            var photoStream = Guid.NewGuid();
            var gpsStream = Guid.NewGuid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register all subscribes and publishes, which only add the information to JobManager
            await jobManager.RegisterSubscribe(tagSourceGrain, tag);
            await jobManager.RegisterSubscribe(photoSourceGrain, photo);
            await jobManager.RegisterSubscribe(gpsSourceGrain, gps);
            await jobManager.RegisterSubscribe(sinkGrain, tagStream);
            await jobManager.RegisterSubscribe(sinkGrain, photoStream);
            await jobManager.RegisterSubscribe(sinkGrain, gpsStream);

            await jobManager.RegisterPublish(tagSourceGrain, tagStream);
            await jobManager.RegisterPublish(photoSourceGrain, photoStream);
            await jobManager.RegisterPublish(gpsSourceGrain, gpsStream);

            // STEP 3: register the operators, which will activate the grain
            await jobManager.RegisterISourceGrain(tagSourceGrain, "SourceGrain", "", "0");
            await jobManager.RegisterISourceGrain(photoSourceGrain, "SourceGrain", "", "0");
            await jobManager.RegisterISourceGrain(gpsSourceGrain, "SourceGrain", "", "0");
            await jobManager.RegisterISinkGrain(sinkGrain, "SinkGrain", "", "0");

            // STEP 4: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var Tag = streamProvider.GetStream<string>(tag, null);
            var Photo = streamProvider.GetStream<string>(photo, null);
            var GPS = streamProvider.GetStream<string>(gps, null);

            // STEP 5: let DataDriver feeds data to streams
            await DataDriver.Run(Photo, Tag, GPS, 1600, 0);
        }

        private static async Task Client1(IClusterClient client)
        {
            //string NameSpace = "StreamProcessing.Grain.Implementation.";

            // STEP 1: create IDs for all grains and streams
            var filterGrain = Guid.NewGuid();
            var sinkGrain = Guid.NewGuid();

            var sourceStream = Guid.NewGuid();
            var filteredStream = Guid.NewGuid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register all subscribes and publishes, which only add the information to JobManager
            await jobManager.RegisterSubscribe(filterGrain, sourceStream);
            await jobManager.RegisterSubscribe(sinkGrain, filteredStream);

            await jobManager.RegisterPublish(filterGrain, filteredStream);
            
            // STEP 3: register the operators, which will activate the grain
            await jobManager.RegisterIFilterGrain(filterGrain, "LargerThanTenFilter", "0", "");
            await jobManager.RegisterISinkGrain(sinkGrain, "SinkGrain", "", "0");

            // STEP 4: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var Stream = streamProvider.GetStream<MyType>(sourceStream, null);

            Random random = new Random();
            Console.Write("Client console: ");
            for (int i = 0; i < 20; ++i)
            {
                long r = random.Next(20);  // Randomly generate twenty numbers between 0 and 19.
                Console.Write(r + " ");      // Output these numbers to Client console.

                Timestamp time = new Timestamp(0);
                var record = new MyType(r.ToString(), "", time);

                // STEP 5: let DataDriver feeds data to streams
                await Stream.OnNextAsync(record);
            }
        }
    }
}