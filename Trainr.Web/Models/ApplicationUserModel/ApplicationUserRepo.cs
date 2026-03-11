using Trainr.Web.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.Models
{
    public class ApplicationUserRepo : IApplicationUserRepo
    {

        private ApplicationDbContext database;

        public ApplicationUserRepo(ApplicationDbContext dbContext)
        {

            this.database = dbContext;

        }

        public Athlete FindAthleteSportTypeAndPosition(string sportType, string athletePosition)
        {

            Athlete athlete = database.Athletes.Where(a => a.sportType == sportType && a.athletePosition == athletePosition).FirstOrDefault();

            return athlete;
            
        }

        public Trainer FindTrainerSportType(string sportType)
        {

            Trainer trainer = database.Trainers.Where(t => t.sportType == sportType).FirstOrDefault();

            return trainer;

        }

        public Task UpdateAthleteSportTypeAndPositionAsync(Athlete athlete)
        {
            database.Athletes.Update(athlete);
            return database.SaveChangesAsync();
        }

        public Task UpdateTrainerSportTypeAsync(Trainer trainer)
        {
            database.Trainers.Update(trainer);
            return database.SaveChangesAsync();
        }

        public List<ApplicationUser> ListAllApplicationUsers()
        {
            List<ApplicationUser> applicationUsers = database.ApplicationUsers
                                    .OrderBy(a => a.lastName)
                                    .ToList<ApplicationUser>();
            return applicationUsers;
        }

        public ApplicationUser FindUser(string id)
        {
            ApplicationUser user =
                database.ApplicationUsers.Find(id);
            return user;
        }
        //JSON
        public string GetCurrentRoles(string id)
        {
            string JSONData = null;

            //LINQ for SQL

            //1. from, 2. where, 3. select (RoleID & RoleName)


            var userRoleList =
                from UR in database.UserRoles
                join R in database.Roles
                on UR.RoleId equals R.Id
                where UR.UserId == id
                select new { R.Id, R.Name };

            JSONData = JsonConvert.SerializeObject(userRoleList);
            return JSONData;
        }

        public string GetAvailableRoles(string id)
        {
            string JSONData = null;

            var availableRoleList =
                from R in database.Roles
                where !
                (
                    from UR in database.UserRoles
                    where UR.UserId == id
                    select UR.RoleId
                ).Contains(R.Id)
                select new { R.Id, R.Name };

            JSONData = JsonConvert.SerializeObject(availableRoleList);


            return JSONData;
        }

        


    }
}
