using Trainr.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.Data
{
    public class DbInitializer
    {
        public static async Task Initialize(IServiceProvider services)
        {
            ApplicationDbContext database = 
                services.GetRequiredService<ApplicationDbContext>();

            UserManager<ApplicationUser> userManager = 
                services.GetRequiredService<UserManager<ApplicationUser>>();

            RoleManager<IdentityRole> roleManager =
                services.GetRequiredService<RoleManager<IdentityRole>>();

            string roleAdmin = "Admin";
            string roleAthlete = "Athlete";
            string roleTrainer = "Trainer";

 
            

            if (!database.Roles.Any())
            {
                IdentityRole role = new IdentityRole(roleAdmin);
                await roleManager.CreateAsync(role);

                role = new IdentityRole(roleAthlete);
                await roleManager.CreateAsync(role);

                role = new IdentityRole(roleTrainer);
                await roleManager.CreateAsync(role);
            }


            if (!database.ApplicationUsers.Any())
            {
                // have to add location to athletes and trainer
                //ADDED TRAINER ID TO CONSTRUCTOR AND THEREFORE ADDED IT INTO EACH TRAINER ASWELL HERE

                ApplicationUser applicationUser = new ApplicationUser("Josh", "Pratt", "JP1111@gmail.com", "(777)-777-7777", "Admin");
                applicationUser.EmailConfirmed = true;
                await userManager.CreateAsync(applicationUser);
                await userManager.AddToRoleAsync(applicationUser, roleAdmin);


                Athlete athlete = new Athlete("Julian", "Williams", "Football", "Jdw0000@gmail.com", "(201)-999-9999", "Athlete1", "Wide Reciever");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);


                athlete = new Athlete("Kam", "Hayer", "Soccer", "kh0000@gmail.com", "(000)-000-0000", "Soccer2017", "Athlete2");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);

                athlete = new Athlete("Jaylan", "Mobely", "Basketball", "Jm0000@gmail.com", "(110)-111-1111", "Athlete3", "Small Foward");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);

                athlete = new Athlete("Marvin", "Jones", "Football", "MW0000@gmail.com", "(108)-999-9999", "Athlete4", "Wide Reciever");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);


                athlete = new Athlete("Joey", "Guy", "Soccer", "JG0000@gmail.com", "(999)-000-0000", "Athlete5", "Striker");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);

                athlete = new Athlete("Juan", "Mas", "Basketball", "JM2222@gmail.com", "(888)-111-1111", "Athlete6", "Small Foward");
                athlete.EmailConfirmed = true;
                await userManager.CreateAsync(athlete);
                await userManager.AddToRoleAsync(athlete, roleAthlete);

                Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);

                trainer = new Trainer("Nanda", "Surendra", "Soccer", "NS1111@gmail.com", "(104)-333-3333", "Trainer2");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);

                trainer = new Trainer("JJ", "Watt", "Basketball", "JW1111@gmail.com", "(101)-101-1010", "Trainer3");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);

                trainer = new Trainer("Jesus", "Love", "Football", "JL1111@gmail.com", "(102)-222-2222", "Trainer4");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);

                trainer = new Trainer("Salman", "Mann", "Soccer", "SM1111@gmail.com", "(103)-333-3333", "Trainer5");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);

                trainer = new Trainer("TJ", "Jackson", "Basketball", "TJ1111@gmail.com", "(101)-101-1010", "Trainer6");
                trainer.EmailConfirmed = true;
                await userManager.CreateAsync(trainer);
                await userManager.AddToRoleAsync(trainer, roleTrainer);
            }

            if (!database.TrainingSessions.Any())
            {
                DateTime sessionStartDate = new DateTime(2019, 10, 1, 5, 00, 00);
                DateTime sessionEndDate = new DateTime(2019, 10, 1, 5, 30, 00);


                string athleteId = database.Athletes.Where(a => a.lastName == "Hayer").FirstOrDefault().Id;
                string trainerId = database.Trainers.Where(t => t.lastName == "Surendra").FirstOrDefault().Id;
                TrainingSession session = new TrainingSession(sessionStartDate,sessionEndDate,athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();

                sessionStartDate = new DateTime(2019, 10, 3, 1, 30, 00);
                sessionEndDate = new DateTime(2019, 10, 3, 2, 00, 00);
                athleteId = database.Athletes.Where(a => a.lastName == "Williams").FirstOrDefault().Id;
                trainerId = database.Trainers.Where(t => t.lastName == "Diaz").FirstOrDefault().Id;
                session = new TrainingSession(sessionStartDate, sessionEndDate,athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();


                sessionStartDate = new DateTime(2019, 10, 5, 8, 30, 00);
                sessionEndDate = new DateTime(2019, 10, 5, 9, 00, 00);
                athleteId = database.Athletes.Where(a => a.lastName == "Mas").FirstOrDefault().Id;
                trainerId = database.Trainers.Where(t => t.lastName == "Jackson").FirstOrDefault().Id;
                session = new TrainingSession(sessionStartDate, sessionEndDate,athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();

                sessionStartDate = new DateTime(2019, 10, 9, 4, 00, 00);
                sessionEndDate = new DateTime(2019, 10, 9, 4, 30, 00);
                athleteId = database.Athletes.Where(a => a.lastName == "Guy").FirstOrDefault().Id;
                trainerId = database.Trainers.Where(t => t.lastName == "Mann").FirstOrDefault().Id;
                session = new TrainingSession(sessionStartDate,sessionEndDate, athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();

                sessionStartDate = new DateTime(2019, 10, 11, 2, 00, 00);
                sessionEndDate = new DateTime(2019, 10, 11, 2, 30, 00);
                athleteId = database.Athletes.Where(a => a.lastName == "Jones").FirstOrDefault().Id;
                trainerId = database.Trainers.Where(t => t.lastName == "Love").FirstOrDefault().Id;
                session = new TrainingSession(sessionStartDate,sessionEndDate, athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();

                sessionStartDate = new DateTime(2019, 10, 13, 9, 30, 00);
                sessionEndDate = new DateTime(2019, 10, 13, 10, 00, 00);
                athleteId = database.Athletes.Where(a => a.lastName == "Mobley").FirstOrDefault().Id;
                trainerId = database.Trainers.Where(t => t.lastName == "Watt").FirstOrDefault().Id;
                session = new TrainingSession(sessionStartDate, sessionEndDate,athleteId, trainerId);
                database.TrainingSessions.Add(session);
                database.SaveChanges();
            }

            if(!database.TrainerSchedules.Any())
            {
                DateTime scheduleStartDate  = new DateTime(2019, 8, 3, 1, 30, 00);
                DateTime scheduleEndDate = new DateTime(2019 ,8,3,2,00,00);
                string trainerId = database.Trainers.Where(t => t.lastName == "Diaz").FirstOrDefault().Id;
                TrainerSchedule trainerSchedule = new TrainerSchedule(scheduleStartDate,scheduleEndDate,trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();

                scheduleStartDate = new DateTime(2019, 8, 1, 5, 00, 00);
                scheduleEndDate = new DateTime(2019, 8, 1, 5, 30, 00);
                trainerId = database.Trainers.Where(t => t.lastName == "Surendra").FirstOrDefault().Id;
                trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();

                scheduleStartDate = new DateTime(2019, 8, 13, 9, 30, 00);
                scheduleEndDate = new DateTime(2019, 8, 13, 10, 00, 00);
                trainerId = database.Trainers.Where(t => t.lastName == "Watt").FirstOrDefault().Id;
                trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();

                scheduleStartDate = new DateTime(2019, 8, 11, 2, 00, 00);
                scheduleEndDate = new DateTime(2019, 8, 11, 2, 30, 00);
                trainerId = database.Trainers.Where(t => t.lastName == "Love").FirstOrDefault().Id;
                trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();

                scheduleStartDate = new DateTime(2019, 8, 9, 4, 00, 00);
                scheduleEndDate = new DateTime(2019, 8, 9, 4, 30, 00);
                trainerId = database.Trainers.Where(t => t.lastName == "Mann").FirstOrDefault().Id;
                trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();

                scheduleStartDate = new DateTime(2019, 8, 5, 8, 30, 00);
                scheduleEndDate = new DateTime(2019, 8, 5, 9, 00, 00);
                trainerId = database.Trainers.Where(t => t.lastName == "Jackson").FirstOrDefault().Id;
                trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainerId);
                database.TrainerSchedules.Add(trainerSchedule);
                database.SaveChanges();
            }
        }

    }
}
