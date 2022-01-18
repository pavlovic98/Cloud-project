
using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    public class ArchiveService : IArchiveService
    {
        /*var activeFlights = _cloudTable.CreateQuery<Flight>().AsEnumerable<Flight>().Where(item => item.PartitionKey.Equals("Flight") && item.Active == true).ToList();*/
        private IReliableStateManager _manager = null;
        private CloudTable _cloudTable = null;
        public ArchiveService(IReliableStateManager manager, CloudTable cloudTable)
        {
            _manager = manager;
            _cloudTable = cloudTable;
        }
        public async Task AddExam(Exam exam)
        {
            Random random = new Random();
            exam.Id = random.Next(10000);
            var exams = await _manager.GetOrAddAsync<IReliableDictionary<int, Exam>>("exams");
            using (var tx = _manager.CreateTransaction())
            {
                await exams.TryAddAsync(tx,exam.Id,exam);   
                await tx.CommitAsync();
                
            }
            ExamEntity examEntity = new ExamEntity(10)
            {
                FirstName = exam.FirstName,
                LastName = exam.LastName,
                Index = exam.Index,
                ProfesorName = exam.ProfesorName,
                ExamName = exam.ExamName,
                ExamPeriod = exam.ExamPeriod,
                Date = exam.Date,
                Time = exam.Time
            };

            /*TableOperation flight = TableOperation.Insert(examEntity);
            try
            {
                var a = await _cloudTable.ExecuteAsync(flight);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }*/
            


            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(exam);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:8359/Home/Publish", data);
        }
    }
}
