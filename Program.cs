using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    partial class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        private static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            TestCalendarAccess().GetAwaiter().GetResult();

            Console.ReadKey();
        }

        private static async Task TestCalendarAccess()
        {
            var serviceClient = AuthenticatedGraphServiceClient.Get(Configuration);
            var events = await serviceClient
                .Users[Configuration["userMailbox"]]
                .Calendar.Events.Request().GetAsync();
        }
    }
}