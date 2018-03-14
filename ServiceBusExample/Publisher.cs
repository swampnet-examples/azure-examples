using Microsoft.Azure.ServiceBus;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ServiceBusExample
{
    class Publisher
    {
		private readonly QueueClient _client;
		private readonly CancellationTokenSource _cancelTokenSource;

		public Publisher(string connectionString)
		{
			_client = new QueueClient(new ServiceBusConnectionStringBuilder(connectionString));
			_cancelTokenSource = new CancellationTokenSource();

			Task.Run(() => SendThread(_cancelTokenSource.Token));
		}


		public Task CloseAsync()
		{
			_cancelTokenSource.Cancel();
			return _client.CloseAsync();
		}


		/// <summary>
		/// Send a bunch of messages every few seconds
		/// </summary>
		/// <param name="token"></param>
		private async void SendThread(CancellationToken token)
		{
			var rnd = new Random();

			try
			{
				int i = 0;

				while (!token.IsCancellationRequested)
				{
					for(int j = 0; j < rnd.Next(1, 10); j++)
					{
						// Create a new message to send to the queue
						string messageBody = $"Message {i}.{j}";

						var message = new Message(Encoding.UTF8.GetBytes(messageBody));

						// Write the body of the message to the console
						Console.WriteLine($"{DateTime.Now.TimeOfDay} - Sending message: {messageBody}");

						// Send the message to the queue
						await _client.SendAsync(message);
					}

					i++;

					await Task.Delay(5000);
				}
			}
			catch (OperationCanceledException)
			{
			}

			Console.WriteLine("Send Complete");
		}
	}
}
