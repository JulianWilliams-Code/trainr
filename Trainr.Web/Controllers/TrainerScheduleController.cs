using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trainr.Web.Data;
using Trainr.Web.Models;
using Trainr.Web.Models.TrainingSessionModel;
using Trainr.Web.ViewModel;
using Trainr.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Trainr.Web.Controllers
{
    public class TrainerScheduleController : Controller
    {
        private ITrainerRepo iTrainerRepo;
        private ITrainerScheduleRepo iTrainerScheduleRepo;
        private IApplicationUserRepo iApplicationUserRepo;
        private ITrainingSessionRepo iTrainingSessionRepo;


        public TrainerScheduleController(ITrainerRepo repo, ITrainerScheduleRepo trainerScheduleRepo, IApplicationUserRepo applicationUserRepo, ITrainingSessionRepo trainingSessionRepo)
        {
            this.iTrainerRepo = repo;
            this.iTrainerScheduleRepo = trainerScheduleRepo;
            this.iApplicationUserRepo = applicationUserRepo;
            this.iTrainingSessionRepo = trainingSessionRepo;
        }

        public void PopulateDropDownList()
        {
            ViewData["TrainerList"] = new SelectList(iTrainerRepo.ListAllAvailableTrainers(), "trainerId", "UserName");
        }

        [HttpGet]
        public IActionResult SearchTrainers()
        {
            PopulateDropDownList();

            SearchTrainersViewModel trainerViewModel = new SearchTrainersViewModel();

            return View(trainerViewModel);
        }

        [HttpPost]
        public IActionResult SearchTrainers(SearchTrainersViewModel trainerViewModel)
        {
            PopulateDropDownList();

            trainerViewModel.trainerScheduleList = SearchTrainersHelper(trainerViewModel);

            return View(trainerViewModel);
        }

        public List<TrainerSchedule> SearchTrainersHelper(SearchTrainersViewModel trainerViewModel)
        {
            List<TrainerSchedule> trainersSchedulesList = iTrainerScheduleRepo.ListOfTrainerSchedules();

            if (trainerViewModel.SportType != null)
            {
                trainersSchedulesList = trainersSchedulesList.Where(t => t.trainer.sportType == trainerViewModel.SportType).ToList();


            }

            return trainersSchedulesList;

        }


        [Authorize(Roles = ("Admin"))]
        [HttpGet]
        public IActionResult AddTrainers()
        {
            PopulateDropDownList();

            return View("~/Views/TrainerSchedule/AddTrainers.cshtml");
        }

        [HttpPost]
        public IActionResult AddTrainers(AddTrainersViewModel trainerViewModel)
        {
            if (ModelState.IsValid)
            {
                //1. Add a Trainer
                Trainer trainer = new Trainer(trainerViewModel.firstName, trainerViewModel.lastName, trainerViewModel.sportType, trainerViewModel.email, trainerViewModel.phoneNumber, trainerViewModel.passWord);
                AddTrainerHelper(trainer).Wait();

                //2. Add Trainer Schedule
                string trainerId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                TrainerSchedule trainerSchedule = new TrainerSchedule(trainerViewModel.StartDateTime, trainerViewModel.EndDateTime, trainerId);
                AddTrainerScheduleHelper(trainerSchedule).Wait();

                return RedirectToAction("SearchTrainers");
            }// end of is valid true
            else
            {
                PopulateDropDownList();
                return View("~/Views/TrainerSchedule/AddTrainers.cshtml"); // returns them back to page if they dont provide the correct infomoration
            }// end of is valid false
        }// end of addtrainer post method

        [Authorize(Roles = ("Admin"))]
        [HttpGet]
        public IActionResult EditTrainer(string TrainerID)
        {

            Trainer trainer = iTrainerRepo.FindTrainer(TrainerID);

            return View("~/Views/TrainerSchedule/EditTrainers.cshtml", trainer);
        }//end of edit trainer post method

        [HttpPost]
        public IActionResult EditTrainer(Trainer trainer)
        {

            iTrainerRepo.EditTrainerAsync(trainer).Wait();

            return RedirectToAction("SearchTrainers");

        } //end of edit trainer post method

        [Authorize(Roles = ("Admin"))]
        [HttpGet]
        public IActionResult ConfirmDeleteTrainers(string TrainerID)
        {

            Trainer trainer = iTrainerRepo.FindTrainer(TrainerID);

            return View("~/Views/TrainerSchedule/ConfirmDeleteTrainers.cshtml", trainer);
        }//end of delete trainer get method

        [HttpPost]
        public IActionResult DeleteTrainer(Trainer trainer)
        {

            iTrainerRepo.DeleteTrainerAsync(trainer).Wait();

            return RedirectToAction("SearchTrainers");
        }//end of delete trainer post method

        [Authorize(Roles = ("Admin"))]
        [HttpGet]
        public IActionResult EditTrainerSchedule(int TrainerScheduleID)
        {

            TrainerSchedule trainerSchedule = iTrainerScheduleRepo.FindTrainerSchedule(TrainerScheduleID);

            return View("~/Views/TrainerSchedule/EditTrainerSchedule.cshtml", trainerSchedule);
        } // end of edit trainer schedule get method

        [HttpPost]
        public IActionResult EditTrainerSchedule(TrainerSchedule trainerSchedule)
        {

            iTrainerScheduleRepo.EditTrainerScheduleAsync(trainerSchedule).Wait();

            return RedirectToAction("SearchTrainers");

        } // end of edit trainer schedule post method

        [Authorize(Roles = ("Admin"))]
        [HttpGet]
        public IActionResult ConfirmDeleteTrainersSchedule(int TrainerScheduleID)
        {

            TrainerSchedule trainerSchedule = iTrainerScheduleRepo.FindTrainerSchedule(TrainerScheduleID);

            return View("~/Views/TrainerSchedule/EditTrainerSchedule.cshtml", trainerSchedule);
        }// end of confirm delete trainer schedule get method 

        [HttpPost]
        public IActionResult DeleteTrainerSchedule(TrainerSchedule trainerSchedule)
        {
            iTrainerScheduleRepo.DeleteTrainerScheduleAsync(trainerSchedule).Wait();

            return RedirectToAction("SearchTrainers");
        }// end of delete trainer schedule post method



        public async Task AddTrainerHelper(Trainer trainer)
        {
            await iTrainerScheduleRepo.AddTrainerAsync(trainer);
        }// end of helper method

        public async Task AddTrainerScheduleHelper(TrainerSchedule trainerSchedule)
        {
            await iTrainerScheduleRepo.AddTrainerScheduleAsync(trainerSchedule);
        }// end of helper method

        
    }
}