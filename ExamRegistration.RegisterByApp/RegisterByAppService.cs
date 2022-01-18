using Common.Interfaces;
using Db.Models;
using System.Threading.Tasks;
using Common.Utils;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace ExamRegistration.RegisterByApp
{
    public class RegisterByAppService : IAppService
    {
        public async Task AddExam(Exam exam)
        {
            var wcfClient = (ServicePartitionClient<WcfCommunicationClient<IArchiveService>>)(
                await WcfClientCreator.Create("fabric:/ExamRegistration/ExamRegistration.Archive", Protocol.TCP, "IArchiveService"));

            await wcfClient.InvokeWithRetryAsync(client => client.Channel.AddExam(exam));

        }

        public async Task GetExams(bool active)
        {
            var wcfClient = (ServicePartitionClient<WcfCommunicationClient<IArchiveService>>)(
                await WcfClientCreator.Create("fabric:/ExamRegistration/ExamRegistration.Archive", Protocol.TCP, "IArchiveService"));

            await wcfClient.InvokeWithRetryAsync(client => client.Channel.GetExams(active));
        }
    }
}
