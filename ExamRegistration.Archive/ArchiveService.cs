
using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    public class ArchiveService : IArchiveService
    {
        /*var activeFlights = _cloudTable.CreateQuery<Flight>().AsEnumerable<Flight>().Where(item => item.PartitionKey.Equals("Flight") && item.Active == true).ToList();*/
        private IReliableStateManager _manager = null;
        private DataRepository _dataRepository = null;
        public ArchiveService(IReliableStateManager manager, DataRepository dataRepository)
        {
            _manager = manager;
            _dataRepository = dataRepository;
        }
        public async Task AddExam(Exam exam)
        {
            var exams = await _manager.GetOrAddAsync<IReliableDictionary<string, Exam>>("exams");
            using (var tx = _manager.CreateTransaction())
            {
                await exams.TryAddAsync(tx,exam.Id,exam);   
                await tx.CommitAsync();
                
            }

            await _dataRepository.PostData(exam);
            


            HttpClient client = new HttpClient();
            var json = JsonConvert.SerializeObject(exam);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:8359/Home/Publish", data);
            
        }

        public async Task GetExams(bool active)
        {
            ExamList examList = new ExamList();
            if (active)
            {
                var exams = await _manager.GetOrAddAsync<IReliableDictionary<string, Exam>>("exams");

                using (var tx = _manager.CreateTransaction())
                {
                    var enumerator = (await exams.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                    try
                    {
                        while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                        {
                            examList.Items.Add(enumerator.Current.Value);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                HttpClient client = new HttpClient();
                var json = JsonConvert.SerializeObject(examList);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("http://localhost:8359/Home/PublishAllActive", data);

            }
            else
            {
                var archivedExams = await _manager.GetOrAddAsync<IReliableDictionary<string, Exam>>("archivedExams");

                using (var tx = _manager.CreateTransaction())
                {
                    var enumerator = (await archivedExams.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                    try
                    {
                        while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                        {
                            examList.Items.Add(enumerator.Current.Value);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                HttpClient client = new HttpClient();
                var json = JsonConvert.SerializeObject(examList);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                await client.PostAsync("http://localhost:8359/Home/PublishAllArchive", data);

            }


        }


        public async Task CheckExams(IReliableDictionary<string, Exam> active, IReliableDictionary<string,Exam> archive)
        {
            using (var tx = _manager.CreateTransaction())
            {
                var enumerator = (await active.CreateEnumerableAsync(tx)).GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync(new System.Threading.CancellationToken()))
                    {
                        DateTime now = DateTime.ParseExact(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        string examTImeString = $"{enumerator.Current.Value.Date} {enumerator.Current.Value.Time}";
                        DateTime examTime = DateTime.ParseExact(examTImeString,"dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                        if(now > examTime)
                        {
                            var item = enumerator.Current.Value;
                            item.Archived = true;
                            await active.TryRemoveAsync(tx, item.Id);
                            await archive.AddAsync(tx,item.Id, item);
                            await _dataRepository.Archive(item.Id);
                            HttpClient client = new HttpClient();
                            var json = JsonConvert.SerializeObject(item);
                            var data = new StringContent(json, Encoding.UTF8, "application/json");
                            await client.PostAsync("http://localhost:8359/Home/PublishArchive", data);

                        }
                    }
                    await tx.CommitAsync();
                }
                catch (Exception e)
                {
                    string s = e.Message;
                    
                }
            }
        }
    }
}
