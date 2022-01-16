
using Common.Interfaces;
using Db.Models;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System.Threading;

namespace ExamRegistration.RegisterByMail
{
    public class RegisterByMailService : IMailService
    {
        private IReliableStateManager _manager = null;
        public RegisterByMailService(IReliableStateManager manager)
        {
            _manager = manager;
            
        }

        
    }
}
