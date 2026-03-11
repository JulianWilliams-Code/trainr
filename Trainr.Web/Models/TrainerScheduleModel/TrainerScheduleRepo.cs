using Trainr.Web.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.Models
{
    public class TrainerScheduleRepo : ITrainerScheduleRepo
    {
        private ApplicationDbContext database;


        public TrainerScheduleRepo(ApplicationDbContext dbContext)
        {
            this.database = dbContext;
        }
        public List<TrainerSchedule> ListOfTrainerSchedules()
        {
            List<TrainerSchedule> trainersSchedulesList = database.TrainerSchedules
                .Include(ts => ts.trainer)
                .Where(ts => ts.isAvailable)
                .ToList<TrainerSchedule>();

            return trainersSchedulesList;
        }

        public Task AddTrainerAsync(Trainer trainer)
        {

            database.Trainers.AddAsync(trainer);
            return database.SaveChangesAsync();


        }

        public Task AddTrainerScheduleAsync(TrainerSchedule trainerSchedule)
        {

            database.TrainerSchedules.AddAsync(trainerSchedule);
            return database.SaveChangesAsync();


        }

        public TrainerSchedule FindTrainerSchedule(int TrainerScheduleID)
        {

            TrainerSchedule trainerSchedule = database.TrainerSchedules.Find(TrainerScheduleID);

            return trainerSchedule;

        }

        public Task EditTrainerScheduleAsync(TrainerSchedule trainerSchedule)
        {
            database.TrainerSchedules.Update(trainerSchedule);

            return database.SaveChangesAsync();
        }

        public Task DeleteTrainerScheduleAsync(TrainerSchedule trainerSchedule)
        {
            database.TrainerSchedules.Remove(trainerSchedule);

            return database.SaveChangesAsync();
        }

        
    }
}

