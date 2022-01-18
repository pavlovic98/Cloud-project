using Common.Interfaces;
using Db.Models;
using ExamRegistration.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ExamRegistration.Web.Controllers
{
    
    public class HomeController : Controller
    {
        public static List<Exam> ActiveExams = new List<Exam>();
        public static List<Exam> ArchivedExams = new List<Exam>();
        private static bool _initActive = true;
        private static bool _initArchive = true;
        
        public IActionResult Index()
        {
            
            return View();
        }
        

        [HttpPost]
        [Route("Home/PublishAllArchive")]
        public void PublishAllArchive([FromBody] ExamList examLsit)
        {
            ArchivedExams = new List<Exam>(examLsit.Items);
        }

        [HttpPost]
        [Route("Home/PublishAllActive")]
        public void PublishAllActive([FromBody] ExamList examLsit)
        {
            ActiveExams = new List<Exam>(examLsit.Items);
        }

        [HttpPost]
        [Route("Home/Publish")]
        public void Publish([FromBody]Exam exam)
        {
            ActiveExams.Add(exam);
        }
        [HttpPost]
        [Route("Home/PublishArchive")]
        public void PublishArchive([FromBody] Exam exam)
        {
            ArchivedExams.Add(exam);
            for (int i = 0; i < ActiveExams.Count; i++)
            {
                if(ActiveExams[i].Id == exam.Id)
                {
                    ActiveExams.RemoveAt(i);
                    break;
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> Submit(Exam exam)
        {
            DateTime dt = DateTime.Parse(exam.Date,CultureInfo.InvariantCulture);
            exam.Date = dt.ToString("dd/MM/yyyy");
            exam.Id = new Random().Next(10000).ToString();
            var myBinding = new NetTcpBinding(SecurityMode.None);
            var myEndpoint = new EndpointAddress("net.tcp://localhost:46001/RegisterByAppEndpoint");
            using (var myChannelFactory = new ChannelFactory<IAppService>(myBinding, myEndpoint))
            {
                IAppService client = null;

                try
                {
                    client = myChannelFactory.CreateChannel();
                    await client.AddExam(exam);
                }
                catch(Exception e)
                {
                    string s = e.Message;
                }
            }

            return RedirectToAction("Active","Home");
        }

        [HttpGet]
        public async Task<IActionResult> Active()
        {
            if (_initActive)
            {
                _initActive = false;
                var myBinding = new NetTcpBinding(SecurityMode.None);
                var myEndpoint = new EndpointAddress("net.tcp://localhost:46001/RegisterByAppEndpoint");
                using (var myChannelFactory = new ChannelFactory<IAppService>(myBinding, myEndpoint))
                {
                    IAppService client = null;

                    try
                    {
                        client = myChannelFactory.CreateChannel();
                        await client.GetExams(true);
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                }
            }
            
            return View(ActiveExams);
        }

        [HttpGet]
        public async Task<IActionResult> Archive()
        {
            if (_initArchive)
            {
                _initArchive = false;
                var myBinding = new NetTcpBinding(SecurityMode.None);
                var myEndpoint = new EndpointAddress("net.tcp://localhost:46001/RegisterByAppEndpoint");
                using (var myChannelFactory = new ChannelFactory<IAppService>(myBinding, myEndpoint))
                {
                    IAppService client = null;

                    try
                    {
                        client = myChannelFactory.CreateChannel();
                        await client.GetExams(false);
                    }
                    catch (Exception e)
                    {
                        string s = e.Message;
                    }
                }
            }
            return View(ArchivedExams);
        }


    }
}
