
using System.ServiceModel;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    [ServiceContract]
    public interface IMailService
    {
        [OperationContract]
        Task Get();
    }
}
