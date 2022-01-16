using System.Runtime.Serialization;

namespace Db.Models
{
    [DataContract]
    public class Exam
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ExamPeriod { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Index { get; set; }
        [DataMember]
        public string ExamName { get; set; }
        [DataMember]
        public string ProfesorName { get; set; }
        [DataMember]
        public string Date { get; set; }
        [DataMember]
        public string Time { get; set; }
        [DataMember]
        public bool Archived { get; set; }

        public Exam()
        {
            Archived = false;
        }

    }
}
