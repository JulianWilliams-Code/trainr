using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Trainr.Web.Models;
using Trainr.Web.Models.TrainingSessionModel;
using Trainr.Web.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Trainr.Web.Controllers
{
    public class TrainingSessionController : Controller
    {
        private ITrainerRepo iTrainerRepo;
        private ITrainerScheduleRepo iTrainerScheduleRepo;
        private IApplicationUserRepo iApplicationUserRepo;
        private ITrainingSessionRepo iTrainingSessionRepo;


        public TrainingSessionController(ITrainerRepo repo, ITrainerScheduleRepo trainerScheduleRepo, IApplicationUserRepo applicationUserRepo, ITrainingSessionRepo trainingSessionRepo)
        {
            this.iTrainerRepo = repo;
            this.iTrainerScheduleRepo = trainerScheduleRepo;
            this.iApplicationUserRepo = applicationUserRepo;
            this.iTrainingSessionRepo = trainingSessionRepo;
        }

        [Authorize(Roles = "Athlete")]
        public IActionResult BookTrainingSession(int trainerScheduleID)
        {
            ViewData["ErrorMessage"] = "None";

            TrainerSchedule trainerSchedule = 
                iTrainerScheduleRepo.FindTrainerSchedule(trainerScheduleID);

            if (!trainerSchedule.isAvailable)
            {

                ViewData["ErrorMessage"] = "Training session requested not available Between" + 
                    trainerSchedule.trainerScheduleStartDateTime.ToString() + " " 
                    + trainerSchedule.trainerScheduleEndDateTime.ToString();
                return View();

            }
            else
            {
                string trainerID = trainerSchedule.trainerId;
                DateTime startSessionTime = trainerSchedule.trainerScheduleStartDateTime;
                DateTime endSessionTime = trainerSchedule.trainerScheduleEndDateTime;
                string athleteID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                TrainingSession trainingSession = new TrainingSession(startSessionTime,
                    endSessionTime, athleteID, trainerID);

                iTrainingSessionRepo.BookTrainingSessionAsync(trainingSession).Wait();

                trainerSchedule.isAvailable = false;
                iTrainerScheduleRepo.EditTrainerScheduleAsync(trainerSchedule).Wait();

                List<TrainingSession> ListOfTrainingSessions = iTrainingSessionRepo.TraniningSessionList();

                return View("ListOfTrainingSessions", ListOfTrainingSessions);

                


            }
            
        }

        //public bool BookTrainingSessionHelper(string athleteID, DateTime requestedTrainingSessionStartDate, DateTime requestedTrainingSessionEndDate)
        //{

        //    bool bookedSession;

        //    // 1. Finds athletes trainer

        //    Trainer trainer = iApplicationUserRepo.FindTrainerForAthlete(athleteID); //use application user

        //    // 2. Checks trainers availability at requested date/time

        //    TrainerSchedule trainerAvailability = iTrainingSessionRepo.CheckTrainerAvailability(trainer.Id, requestedTrainingSessionStartDate,requestedTrainingSessionEndDate);

        //    if (trainerAvailability != null)
        //    {

        //        trainerAvailability.trainerId = athleteID;
        //        trainerAvailability.isAvailable = false;

        //        //3a. If available, book session

        //        iTrainingSessionRepo.BookTrainingSessionAsync(trainerAvailability).Wait();
        //        bookedSession = true;

        //    }
        //    else
        //    {
        //        //3b. If not available, session not made
        //        bookedSession = false;

        //    }

        //    return bookedSession;

        //}

        //public IActionResult BookTrainingSession(int trainerScheduleID)
        //{
        //    /*
            

        //    string trainerID = trainerSchedule.trainerId;            
        //    DateTime startSessionTime = trainerSchedule.trainerScheduleStartDateTime;
        //    DateTime endSessionTime = trainerSchedule.trainerScheduleEndDateTime;
        //    string athleteID = from login;

        //    TrainingSession trainingSession =
        //        new TrainingSession(startSessionTime, endSessionTime, athleteID,
        //        trainerID);
        //    database.TrainingSessions.Add(trainingSession);
        //    */
        //    //make the trainer unavailable for this time.


        //    return View();//Confirm Training Session appointment to Athlete
        //}
    }
}