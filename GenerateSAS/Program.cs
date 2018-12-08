using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;

namespace GenerateSAS
{
    class Program
    {
        static void Main(string[] args)
        {
            var token = GetContainerSasToken(
                GetContainer("test-sas"), 
                "test-access-policy", 
                "5.65.214.182");

            /*
             * https://swampnet.blob.core.windows.net/test-sas/test.png?sv=2018-03-28&sr=c&si=test-access-policy&sig=pbD25IB4rR8XFqJUKZDsIT4VkSAO1KKvvTxOuusG324%3D&spr=https&sip=5.65.214.182
             */
        }

        private static string GetContainerSasToken(CloudBlobContainer container, string storedPolicyName, string ipAddressRange)
        {
            string sasContainerToken;

            // Generate the shared access signature on the container. In this case, all of the constraints for the
            // shared access signature are specified on the stored access policy, which is provided by name.
            // It is also possible to specify some constraints on an ad-hoc SAS and others on the stored access policy.
            sasContainerToken = container.GetSharedAccessSignature(null, storedPolicyName, SharedAccessProtocol.HttpsOnly, new IPAddressOrRange(ipAddressRange));

            Console.WriteLine("SAS for blob container (stored access policy): {0}", sasContainerToken);
            Console.WriteLine();


            // Return the URI string for the container, including the SAS token.
            return sasContainerToken;
        }


        private static CloudBlobContainer GetContainer(string name)
        {
            var storageAccount = GetStorageAccount();

            // Create the CloudBlobClient that represents the Blob storage endpoint for the storage account.
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();

            // Get container reference
            var container = cloudBlobClient.GetContainerReference(name);

            container.CreateIfNotExistsAsync().Wait();

            return container;
        }


        private static CloudStorageAccount GetStorageAccount()
        {
            string connectionString = @"<connection-string>";

            CloudStorageAccount storageAccount = null;

            if (!CloudStorageAccount.TryParse(connectionString, out storageAccount))
            {
                throw new InvalidOperationException("Invalid blob storage connection string");
            }

            return storageAccount;
        }
    }
}
