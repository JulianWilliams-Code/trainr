using Trainr.Web.Data;
using Trainr.Web.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.Models.TrainingSessionModel
{
    public class TrainingSessionRepo : ITrainingSessionRepo
    {
        private ApplicationDbContext database;

        public TrainingSessionRepo(ApplicationDbContext dbContext)
        {
            this.database = dbContext;
            
        }

        public Task BookTrainingSessionAsync(TrainingSession trainingSession)
        {
            database.TrainingSessions.AddAsync(trainingSession);
            return database.SaveChangesAsync();
        }

        public List<TrainingSession> TraniningSessionList()
        {
            List<TrainingSession> TraniningSessionList = database.TrainingSessions
                .Include(ts => ts.trainer)
                .Include(ts => ts.athlete)
                .ToList<TrainingSession>();

            return TraniningSessionList;
        }

        public List<BookingDataViewModel> GetBookingData()
        {
            var bookingData =
                from TS in database.TrainingSessions
                join T in database.Trainers
                on TS.trainerId equals T.Id
                join A in database.Athletes
                on TS.athleteId equals A.Id
                select new BookingDataViewModel
                {
                    trainerFullName = T.fullName,
                    sportType = T.sportType
                };

            List<BookingDataViewModel> bookingList =
                bookingData.ToList();

            return bookingList;
        }
        public string GetBookedSessionsForAllTrainers()
        {
            string jsonData = null;

            List<BookingDataViewModel> bookingList = GetBookingData();

            var result = from data in bookingList
                         group data by new { data.trainerFullName }
                         into bookingGroup
                         select new BookingDataViewModel
                         {
                             trainerFullName = bookingGroup.Key.trainerFullName,
                             totalBookedSessions = bookingGroup.Count()
                         };
            result = result.OrderByDescending(r => r.totalBookedSessions);

            jsonData = JsonConvert.SerializeObject(result);

            return jsonData;
        }

        public string GetBookedSessionsForOneTrainer(string Name)
        {
            string jsonData = null;

            List<BookingDataViewModel> bookingList =
                GetBookingData().Where(l => l.trainerFullName == Name)
                .ToList();

            var result = from data in bookingList
                         group data by new { data.sportType }
                         into bookingGroup
                         select new BookingDataViewModel
                         {
                             sportType = bookingGroup.Key.sportType,
                             totalBookedSessions = bookingGroup.Count()
                         };
            result = result.OrderByDescending(r => r.totalBookedSessions);

            jsonData = JsonConvert.SerializeObject(result);

            return jsonData;
        }


    }
}
