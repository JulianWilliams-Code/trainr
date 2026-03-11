using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Trainr.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Trainr.Web.Models.TrainingSessionModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trainr.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITrainingSessionRepo iTraingingSessionRepo;
        private readonly ITrainerRepo iTrainerRepo;



        public HomeController(ILogger<HomeController> logger,ITrainingSessionRepo trainingSessionRepo, ITrainerRepo trainerRepo)
        {
            _logger = logger;
            this.iTraingingSessionRepo = trainingSessionRepo;
            this.iTrainerRepo = trainerRepo;
        }

        public string GetBookedSessionsForAllTrainers()
        {
            string JSONData = iTraingingSessionRepo.GetBookedSessionsForAllTrainers();
            return JSONData;
        }

        public string GetBookedSessionsForOneTrainer(string id)
        {
            string JSONData = iTraingingSessionRepo.GetBookedSessionsForOneTrainer(id);
            return JSONData;
        }

        public IActionResult Index()
        {
            ViewData["TrainerList"] = new SelectList(iTrainerRepo.ListAllAvailableTrainers(), 
                "fullName", "fullName");
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
