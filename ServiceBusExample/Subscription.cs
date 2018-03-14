using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusExample
{
    class Subscription
    {
		private readonly QueueClient _client;

		public Subscription(string connectionString)
		{
			_client = new QueueClient(new ServiceBusConnectionStringBuilder(connectionString));

			// Register the queue message handler and receive messages in a loop

			// Register the function that processes messages.
			_client.RegisterMessageHandler(
				ProcessMessagesAsync, 
				new MessageHandlerOptions(ExceptionReceivedHandler)
				{
					// Maximum number of concurrent calls to the callback ProcessMessagesAsync(), set to 1 for simplicity.
					// Set it according to how many messages the application wants to process in parallel.
					MaxConcurrentCalls = 1,

					// Indicates whether the message pump should automatically complete the messages after returning from user callback.
					// False below indicates the complete operation is handled by the user callback as in ProcessMessagesAsync().
					AutoComplete = false
				});
		}


		public Task CloseAsync()
		{
			return _client.CloseAsync();
		}


		private async Task ProcessMessagesAsync(Message message, CancellationToken token)
		{
			// Process the message.
			Console.WriteLine($"{DateTime.Now.TimeOfDay} - Received message: SequenceNumber:{message.SystemProperties.SequenceNumber} Body:{Encoding.UTF8.GetString(message.Body)}");

			// Complete the message so that it is not received again.
			// This can be done only if the queue Client is created in ReceiveMode.PeekLock mode (which is the default).
			await _client.CompleteAsync(message.SystemProperties.LockToken);
			//await _client.DeadLetterAsync(message.SystemProperties.LockToken);

			// Note: Use the cancellationToken passed as necessary to determine if the queueClient has already been closed.
			// If queueClient has already been closed, you can choose to not call CompleteAsync() or AbandonAsync() etc.
			// to avoid unnecessary exceptions.
		}


		// Use this handler to examine the exceptions received on the message pump.
		static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
		{
			Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
			var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
			Console.WriteLine("Exception context for troubleshooting:");
			Console.WriteLine($"- Endpoint: {context.Endpoint}");
			Console.WriteLine($"- Entity Path: {context.EntityPath}");
			Console.WriteLine($"- Executing Action: {context.Action}");
			return Task.CompletedTask;
		}
	}
}
