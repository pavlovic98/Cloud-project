using Common.Interfaces;
using Db.Models;
using ExamRegistration.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ExamRegistration.Web.Controllers
{
    
    public class HomeController : Controller
    {
        public static List<Exam> ActiveExams = new List<Exam>();
        public static List<Exam> ArchivedExams = new List<Exam>();
        
        public IActionResult Index()
        {
            
            return View();
        }
        
        
        [HttpPost]
        [Route("Home/Publish")]
        public IActionResult Publish([FromBody]Exam exam)
        {
            ActiveExams.Add(exam);
            return RedirectToAction("Active","Home");
        }


        [HttpPost]
        public IActionResult Submit(Exam exam)
        {
            
            var myBinding = new NetTcpBinding(SecurityMode.None);
            var myEndpoint = new EndpointAddress("net.tcp://localhost:46001/RegisterByAppEndpoint");
            using (var myChannelFactory = new ChannelFactory<IAppService>(myBinding, myEndpoint))
            {
                IAppService client = null;

                try
                {
                    client = myChannelFactory.CreateChannel();
                    client.AddExam(exam).Wait();
                }
                catch(Exception e)
                {
                    string s = e.Message;
                }
            }

            return RedirectToAction("Active","Home");
        }

        [HttpGet]
        public IActionResult Active()
        {
            return View(ActiveExams);
        }

        [HttpGet]
        public IActionResult Archive()
        {
            return View(ArchivedExams);
        }


    }
}
