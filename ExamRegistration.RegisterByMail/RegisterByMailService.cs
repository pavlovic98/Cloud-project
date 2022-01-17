
using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace ExamRegistration.RegisterByMail
{
    public class RegisterByMailService : IMailService
    {
        private IReliableStateManager _manager = null;
        public RegisterByMailService(IReliableStateManager manager)
        {
            _manager = manager;
            
        }

        public async Task Get()
        {
            throw new System.NotImplementedException();
        }
    }
}
