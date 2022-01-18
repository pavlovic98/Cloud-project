using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Db.Models
{
    [DataContract]
    public class ExamList
    {
        [DataMember]
        public List<Exam> Items { get; set; } = new List<Exam>(); 
    }
}
