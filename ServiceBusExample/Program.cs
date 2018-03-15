using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusExample
{
    class Program
    {
		public static IConfigurationRoot Configuration { get; set; }

		/*
		 * 
		 - Set up container
		 - Add Serilog

		 */

		static void Main(string[] args)
		{
			var devEnvironmentVariable = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
			var isDevelopment = string.IsNullOrEmpty(devEnvironmentVariable) || string.Equals(devEnvironmentVariable, "development", StringComparison.OrdinalIgnoreCase);
			//Determines the working environment as IHostingEnvironment is unavailable in a console app

			var builder = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
				.AddEnvironmentVariables();

			// only add secrets in development
			if (isDevelopment)
			{
				builder.AddUserSecrets(typeof(Program).Assembly);
			}

			Configuration = builder.Build();

			var pub = new Publisher(Configuration["service-bus:connectionString"]);
			var sub = new Subscription(Configuration["service-bus:connectionString"]);

			Console.WriteLine("key");
			Console.ReadKey();

			pub.CloseAsync().Wait();
			sub.CloseAsync().Wait();
		}
	}
}
