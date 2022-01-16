
using Db.Models;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IAppService
    {
        [OperationContract]
        Task AddExam(Exam exam); 
    }
}
