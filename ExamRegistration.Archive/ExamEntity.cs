using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExamRegistration.Archive
{
    class ExamEntity : TableEntity
    {
        public int Id { get; set; }
        public string ExamPeriod { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Index { get; set; }
        public string ExamName { get; set; }
        public string ProfesorName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public bool Archived { get; set; }

        public ExamEntity(int id)
        {
            PartitionKey = "Exam";
            RowKey = id.ToString();
            Archived = false;
        }
        public ExamEntity()
        {

        }
    }
}
