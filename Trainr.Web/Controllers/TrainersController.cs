using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trainr.Web.Data;
using Trainr.Web.Models;
using Trainr.Web.ViewModel;
using Trainr.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Trainr.Web.Controllers
{
    public class TrainersController : Controller
    {

        private ITrainerRepo iTrainerRepo;
        
        public TrainersController(ITrainerRepo repo)
        {
            this.iTrainerRepo = repo;
        }

        public IActionResult ListAllAvailableTrainers()
        {
            List<Trainer> trainerList = ListAllAvailableTrainersHelper();

            return View("~/Views/Trainers/ListAllAvailableTrainers.cshtml", trainerList);
        }

        public List<Trainer> ListAllAvailableTrainersHelper()
        {
            List<Trainer> trainersList = iTrainerRepo.ListAllAvailableTrainers();

            return trainersList;
        }
    }
}