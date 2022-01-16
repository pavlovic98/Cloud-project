
using Common.Interfaces;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Fabric;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace Common.Utils
{
    public enum Protocol
    {
        TCP,
        HTTP
    }
    public class WcfClientCreator
    {
        public async static Task<Object> Create(string address, Protocol protocol, string contract)
        {
            FabricClient fabricClient = new FabricClient();
            int partiotionNumber = (await fabricClient.QueryManager.GetPartitionListAsync(new Uri(address))).Count;
            Binding binding = null;
            if (protocol == Protocol.TCP)
            {
                binding = WcfUtility.CreateTcpClientBinding();
            }
            if (contract == "IArchiveService")
            {
                ServicePartitionClient<WcfCommunicationClient<IArchiveService>> servicePartitionClient = null;

                servicePartitionClient = new ServicePartitionClient<WcfCommunicationClient<IArchiveService>>(
                    new WcfCommunicationClientFactory<IArchiveService>(clientBinding: binding),
                    new Uri(address),
                    new ServicePartitionKey(0));
                return servicePartitionClient;
            }
      
            return null;
        }
    }
}
