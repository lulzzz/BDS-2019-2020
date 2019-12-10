using Orleans;
using System;
using System.Threading.Tasks;
using Grains.Interfaces;
using Functions.Implementations;
using Microsoft.Extensions.Logging;
using Orleans.Configuration;

namespace Client
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
                    await DoClientWork(client);
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
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            var grainId = Guid.NewGuid();
            var sampleGrain = client.GetGrain<ISample>(grainId);
            var sinkId = Guid.NewGuid();
            await sampleGrain.Init(sinkId, new LargerThanTen());
            Random random = new Random();
            for (int i = 0; i < 20; i++)
            {
                await sampleGrain.Process<long>(random.Next(20));
            }
        }
    }
}