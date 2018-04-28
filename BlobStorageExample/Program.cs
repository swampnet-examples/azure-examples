using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;

namespace BlobStorageExample
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine("Azure Blob storage");
			Console.WriteLine();

			ProcessAsync().GetAwaiter().GetResult();

			Console.WriteLine("key");
			Console.ReadKey();
		}


		private static async Task ProcessAsync()
		{
			CloudStorageAccount storageAccount = null;

			// >setx storageconnectionstring "<yourconnectionstring>"
			string storageConnectionString = Environment.GetEnvironmentVariable("storageconnectionstring");

			if (CloudStorageAccount.TryParse(storageConnectionString, out storageAccount))
			{
				try
				{
					// Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
					var cloudBlobClient = storageAccount.CreateCloudBlobClient();

					// Get container reference
					var container = cloudBlobClient.GetContainerReference("test");

					// Create container
					await CreateContainer(container);

					// Get a reference to the blob address, then upload the file to the blob.
					var blobRef = "test-" + Guid.NewGuid().ToString() + ".jpg";
					var blob = container.GetBlockBlobReference(blobRef);
					await blob.UploadFromFileAsync(@"Data\Test.jpg");
					Console.WriteLine("uploaded " + blobRef);

					// List the blobs in the container.
					await List(container);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex);
				}
			}
		}

		private static async Task CreateContainer(CloudBlobContainer cloudBlobContainer)
		{
			if (await cloudBlobContainer.CreateIfNotExistsAsync())
			{
				Console.WriteLine("Created container '{0}'", cloudBlobContainer.Name);
				Console.WriteLine();

				// Set the permissions so the blobs are public. 
				BlobContainerPermissions permissions = new BlobContainerPermissions
				{
					PublicAccess = BlobContainerPublicAccessType.Blob
				};
				await cloudBlobContainer.SetPermissionsAsync(permissions);
			}
		}

		private static async Task List(CloudBlobContainer cloudBlobContainer)
		{
			Console.WriteLine("Listing blobs in container.");
			BlobContinuationToken blobContinuationToken = null;
			do
			{
				var results = await cloudBlobContainer.ListBlobsSegmentedAsync(null, blobContinuationToken);

				// Get the value of the continuation token returned by the listing call.
				blobContinuationToken = results.ContinuationToken;
				foreach (IListBlobItem item in results.Results)
				{
					Console.WriteLine(item.Uri);
				}
			} while (blobContinuationToken != null); // Loop while the continuation token is not null.
			Console.WriteLine();
		}
	}
}
