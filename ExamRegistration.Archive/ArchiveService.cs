
using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    public class ArchiveService : IArchiveService
    {
        private IReliableStateManager _manager = null;

        public ArchiveService(IReliableStateManager manager)
        {
            _manager = manager;
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

            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(exam);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:8359/Home/Publish", data);
        }
    }
}
