using Common.Interfaces;
using Common.Utils;
using Db.Models;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRegistration.RegisterByMail
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class RegisterByMail : StatefulService
    {
        private RegisterByMailService _mailService;
        private MailRepository _mailRepository = null;
        private string _email = "exam.reg.ftn@gmail.com";
        private string _password = "examregistration12";

        public RegisterByMail(StatefulServiceContext context)
            : base(context)
        {
            _mailService = new RegisterByMailService(this.StateManager);
            _mailRepository = new MailRepository("imap.gmail.com", 993, true, _email, _password);
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
                new ServiceReplicaListener(contex=>this.CreateServiceInstanceListeners(contex), "RegisterByMailEndpoint")
            };
        }

        private ICommunicationListener CreateServiceInstanceListeners(StatefulServiceContext context)
        {
            string host = context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = context.CodePackageActivationContext.GetEndpoint("RegisterByMailEndpoint");
            int port = endpointConfig.Port;
            var scheme = endpointConfig.Protocol.ToString();
            string uri = string.Format(CultureInfo.InvariantCulture, "net.{0}://{1}:{2}/RegisterByMailEndpoint", scheme, host, port);

            var listener = new WcfCommunicationListener<IMailService>(
                serviceContext: context,
                wcfServiceObject: _mailService,
                listenerBinding: WcfUtility.CreateTcpListenerBinding(maxMessageSize: 1024 * 1024 * 1024),
                address: new EndpointAddress(uri)
                );

            ServiceEventSource.Current.Message("Napravljen listener!");

            return listener;
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

                await GetMails(exams);
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }

        }

        private async Task GetMails(IReliableDictionary<string, Exam> exams)
        {
            var allEmails = _mailRepository.GetAllMails();
            foreach (var item in allEmails)
            {
                string[] data = item.Item2.Replace("\n", "").Replace("\r", "").Split(',');
                Exam exam = new Exam
                {
                    FirstName = data[0],
                    LastName = data[1],
                    Index = data[2],
                    ExamName = data[3],
                    ProfesorName = data[4],
                    Date = data[5],
                    Time = data[6],
                    ExamPeriod = data[7]
                };

                using (var tx = this.StateManager.CreateTransaction())
                {
                    Exam exist = (await exams.TryGetValueAsync(tx, item.Item1)).Value;
                    if (exist == null)
                    {
                        await exams.TryAddAsync(tx, item.Item1, exam);
                        await tx.CommitAsync();
                        var wcfClient = (ServicePartitionClient<WcfCommunicationClient<IArchiveService>>)(
                        await WcfClientCreator.Create("fabric:/ExamRegistration/ExamRegistration.Archive", Protocol.TCP, "IArchiveService"));

                        await wcfClient.InvokeWithRetryAsync(client => client.Channel.AddExam(exam));
                    }

                }
            }


        }
    }
}
