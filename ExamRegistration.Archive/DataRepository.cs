using Db.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    public class DataRepository
    {
        private CloudStorageAccount _cloudStorageAccount = null;
        private CloudTable _cloudTable = null;

        public DataRepository()
        {
            _cloudStorageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["ConnectionString"]);
            CloudTableClient cloudTableClient = new CloudTableClient(new Uri(_cloudStorageAccount.TableEndpoint.AbsoluteUri), _cloudStorageAccount.Credentials);
            _cloudTable = cloudTableClient.GetTableReference("Exams");
            _cloudTable.CreateIfNotExists();
        }

        public async Task PostData(Exam exam)
        {
            ExamEntity examEntity = new ExamEntity(exam.Id)
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


            TableOperation add = TableOperation.Insert(examEntity);
            try
            {
                var a = await _cloudTable.ExecuteAsync(add);
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }

        public async Task Archive(string id)
        {
            var results = from g in _cloudTable.CreateQuery<ExamEntity>()
                          where g.PartitionKey == "Exam" && g.RowKey == id
                          select g;
            ExamEntity tableEntity = results.SingleOrDefault();
            tableEntity.Archived = true;

            TableOperation edit = TableOperation.Replace(tableEntity);
            await _cloudTable.ExecuteAsync(edit);
        }
        public async Task<IEnumerable<Exam>> RetrieveAll()
        {
            var results = from g in _cloudTable.CreateQuery<ExamEntity>()
                          where g.PartitionKey == "Exam"
                          select g;

            List<Exam> retVal = new List<Exam>();
            foreach (var item in results)
            {
                retVal.Add(new Exam 
                {
                    Id = item.RowKey,
                    FirstName = item.FirstName,
                    LastName = item.LastName,
                    Index = item.Index,
                    ExamName = item.ExamName,
                    ProfesorName = item.ProfesorName,
                    ExamPeriod = item.ExamPeriod,
                    Date = item.Date,
                    Time = item.Time,
                    Archived = item.Archived
                });
            }
            return retVal;
        }

    }
}
