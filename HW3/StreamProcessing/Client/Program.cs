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
                    await Client1(client);
                    //await Client2(client);
                    //await Client3(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
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
                .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(ISourceGrain).Assembly).WithReferences()
                )
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("SMSProvider")
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task Client3(IClusterClient client)
        {
            // STEP 1: create IDs for all grains and streams
            var tagSourceGrain = new Guid();
            var photoSourceGrain = new Guid();
            var tagFilterGrain = new Guid();
            var windowJoinGrain = new Guid();
            var sinkGrain = new Guid();

            var SourceTagStream = new Guid();
            var SourcePhotoStream = new Guid();
            var TagStream = new Guid();
            var PhotoStream = new Guid();
            var FilteredTagStream = new Guid();
            var JoinedStream = new Guid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register all subscribes and publishes, which only add the information to JobManager 
            jobManager.registerSubscribe(tagSourceGrain, SourceTagStream);
            jobManager.registerSubscribe(photoSourceGrain, SourcePhotoStream);
            jobManager.registerSubscribe(tagFilterGrain, TagStream);
            jobManager.registerSubscribe(windowJoinGrain, FilteredTagStream);
            jobManager.registerSubscribe(windowJoinGrain, PhotoStream);
            jobManager.registerSubscribe(sinkGrain, JoinedStream);

            jobManager.registerPublish(tagSourceGrain, TagStream);
            jobManager.registerPublish(photoSourceGrain, PhotoStream);
            jobManager.registerPublish(tagFilterGrain, FilteredTagStream);
            jobManager.registerPublish(windowJoinGrain, JoinedStream);

            // STEP 3: register the operators, which will activate the grain
            // string 1: user defined function
            // string 2 & 3: when the operator emits an event, which columns should be put to new Key and Value
            jobManager.registerISourceGrain(tagSourceGrain, "SourceGrain", "0", "1");
            jobManager.registerISourceGrain(photoSourceGrain, "SourceGrain", "0", "1 2 3");
            jobManager.registerIFilterGrain(tagFilterGrain, "LargerThanTenFilter", "0", "1");
            // for Join Grain, string 2 defines which column is the key that user wants to join on
            // string 3 defines which columns that user wants to keep in the result after join operation
            jobManager.registerIJoinGrain(windowJoinGrain, "WindowJoinGrain", "0, 0", "1, 2 3");
            jobManager.registerISinkGrain(sinkGrain, "SinkGrain", "", "");

            // STEP 4: register window informations
            long window_length = 15000;
            long window_slide = 5000;
            long delay = 0;
            jobManager.registerWindow(windowJoinGrain, window_length, window_slide);
            jobManager.registerAllowedDelay(delay);

            // STEP 5: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var tag = streamProvider.GetStream<string>(SourceTagStream, "");
            var photo = streamProvider.GetStream<string>(SourcePhotoStream, "");
            var gps = streamProvider.GetStream<string>(new Guid(), "");

            // STEP 6: let DataDriver feeds data to streams
            await DataDriver.Run(photo, tag, gps, 1600, 0);
        }

        private static async Task Client2(IClusterClient client)
        {
            // STEP 1: create IDs for all grains and streams
            var tagSourceGrain = new Guid();
            var photoSourceGrain = new Guid();
            var gpsSourceGrain = new Guid();
            var sinkGrain = new Guid();

            var tag = new Guid();
            var photo = new Guid();
            var gps = new Guid();
            var tagStream = new Guid();
            var photoStream = new Guid();
            var gpsStream = new Guid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register all subscribes and publishes, which only add the information to JobManager
            jobManager.registerSubscribe(tagSourceGrain, tag);
            jobManager.registerSubscribe(photoSourceGrain, photo);
            jobManager.registerSubscribe(gpsSourceGrain, gps);
            jobManager.registerSubscribe(sinkGrain, tagStream);
            jobManager.registerSubscribe(sinkGrain, photoStream);
            jobManager.registerSubscribe(sinkGrain, gpsStream);

            jobManager.registerPublish(tagSourceGrain, tagStream);
            jobManager.registerPublish(photoSourceGrain, photoStream);
            jobManager.registerPublish(gpsSourceGrain, gpsStream);

            // STEP 3: register the operators, which will activate the grain
            jobManager.registerISourceGrain(tagSourceGrain, "SourceGrain", "", "all");
            jobManager.registerISourceGrain(photoSourceGrain, "SourceGrain", "", "all");
            jobManager.registerISourceGrain(gpsSourceGrain, "SourceGrain", "", "all");
            jobManager.registerISinkGrain(sinkGrain, "SinkGrain", "", "all");

            // STEP 4: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var Tag = streamProvider.GetStream<string>(tag, "");
            var Photo = streamProvider.GetStream<string>(photo, "");
            var GPS = streamProvider.GetStream<string>(gps, "");

            // STEP 5: let DataDriver feeds data to streams
            await DataDriver.Run(Tag, Photo, GPS, 1600, 0);
        }

        private static async Task Client1(IClusterClient client)
        {
            // The code below shows how to specify an exact grain class which implements the IFilter interface

            // STEP 1: create IDs for all grains and streams
            var filterGrain = new Guid();
            var sinkGrain = new Guid();

            var sourceStream = new Guid();
            var filteredStream = new Guid();

            var jobManager = client.GetGrain<IJobManagerGrain>(0, "JobManager");

            // STEP 2: register all subscribes and publishes, which only add the information to JobManager
            jobManager.registerSubscribe(filterGrain, sourceStream);
            jobManager.registerSubscribe(sinkGrain, filteredStream);

            jobManager.registerPublish(filterGrain, filteredStream);

            // STEP 3: register the operators, which will activate the grain
            jobManager.registerIFilterGrain(filterGrain, "LargerThanTen", "", "all");
            jobManager.registerISinkGrain(sinkGrain, "SinkGrain", "", "all");

            // STEP 4: activate the streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var Stream = streamProvider.GetStream<MyType>(sourceStream, "");

            Random random = new Random();
            for (int i = 0; i < 20; ++i)
            {
                long r = random.Next(20);  // Randomly generate twenty numbers between 0 and 19.
                Console.WriteLine(r);      // Output these numbers to Client console.

                Timestamp time = new Timestamp(0);
                var record = new MyType(r.ToString(), "", time);

                // STEP 5: let DataDriver feeds data to streams
                await Stream.OnNextAsync(record);
            }
        }
    }


}