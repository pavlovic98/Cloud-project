
using Common.Interfaces;
using Common.Utils;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRegistration.RegisterByMail
{
    public class RegisterByMailService : IMailService
    {
        private IReliableStateManager _manager = null;
        private MailRepository _mailRepository = null;
        public RegisterByMailService(IReliableStateManager manager,MailRepository mailRepository)
        {
            _manager = manager;
            _mailRepository = mailRepository;   
        }

        public async Task Get()
        {
            throw new System.NotImplementedException();
        }

        public async Task GetMails(IReliableDictionary<string, Exam> exams)
        {
            var allEmails = _mailRepository.GetAllMails();
            foreach (var item in allEmails)
            {
                string[] data = item.Item2.Replace("\n", "").Replace("\r", "").Split(',');
                if (data.Length == 8 && item.Item1.Split('-').Length == 4)
                {
                    Exam exam = new Exam
                    {
                        Id = new Random().Next(10000).ToString(),
                        FirstName = data[0],
                        LastName = data[1],
                        Index = data[2],
                        ExamName = data[3],
                        ProfesorName = data[4],
                        Date = data[5],
                        Time = data[6],
                        ExamPeriod = data[7]
                    };

                    using (var tx = _manager.CreateTransaction())
                    {
                        Exam exist = (await exams.TryGetValueAsync(tx, exam.Id)).Value;
                        if (exist == null)
                        {
                            await exams.TryAddAsync(tx, item.Item1, exam);
                            await tx.CommitAsync();
                            var wcfClient = (ServicePartitionClient<WcfCommunicationClient<IArchiveService>>)(
                            await WcfClientCreator.Create("fabric:/ExamRegistration/ExamRegistration.Archive", Protocol.TCP, "IArchiveService",1));

                            await wcfClient.InvokeWithRetryAsync(client => client.Channel.AddExam(exam));
                        }

                    }
                }
            }


        }
    }
}
