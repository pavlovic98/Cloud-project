using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Fabric;
using System.Fabric.Description;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class Archive : StatefulService
    {
        private ArchiveService _archiveService;
        private DataRepository _dataRepository;
        
        public Archive(StatefulServiceContext context)
            : base(context)
        {
            _dataRepository = new DataRepository();
            _archiveService = new ArchiveService(this.StateManager, _dataRepository);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new List<ServiceReplicaListener>(1)
            {
                new ServiceReplicaListener(contex=>this.CreateServiceInstanceListeners(contex), "ArchiveEndpoint")
            };
        }

        private ICommunicationListener CreateServiceInstanceListeners(StatefulServiceContext context)
        {
            /*string host = context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("ArchiveEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/ArchiveEndpoint", scheme, host, port);

            var listener = new WcfCommunicationListener<IArchiveService>(
                serviceContext: context,
                wcfServiceObject: _archiveService,
                listenerBinding: WcfUtility.CreateTcpListenerBinding(maxMessageSize: 1024 * 1024 * 1024),
                address: new EndpointAddress(uri)
                );

            ServiceEventSource.Current.Message("Napravljen listener!");

            return listener;*/
            EndpointResourceDescription internalEndpoint = context.CodePackageActivationContext.GetEndpoint("ArchiveEndpoint");
            string uriPrefix = String.Format(
                   "{0}://+:{1}/{2}/{3}-{4}/",
                   internalEndpoint.Protocol,
                   internalEndpoint.Port,
                   context.PartitionId,
                   context.ReplicaOrInstanceId,
                   Guid.NewGuid());

            string nodeIP = FabricRuntime.GetNodeContext().IPAddressOrFQDN;

            string uriPublished = uriPrefix.Replace("+", nodeIP);
            //return new HttpCommunicationListener(uriPrefix, uriPublished, this.ProcessInternalRequest);
            return new WcfCommunicationListener<IArchiveService>(context, _archiveService, WcfUtility.CreateTcpListenerBinding(maxMessageSize: 1024 * 1024 * 1024), uriPrefix);
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");
            var exams = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Exam>>("exams");
            var archivedExams = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, Exam>>("archivedExams");

            var retExams = await _dataRepository.RetrieveAll();
            using (var tx = this.StateManager.CreateTransaction())
            {
                foreach (var item in retExams)
                {
                    if (!item.Archived)
                    {
                        await exams.AddAsync(tx, item.Id,item);
                    }
                    else
                    {
                        await archivedExams.AddAsync(tx, item.Id, item);
                    }
                }
                await tx.CommitAsync();
            }

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }
                await _archiveService.CheckExams(exams,archivedExams);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }
}
