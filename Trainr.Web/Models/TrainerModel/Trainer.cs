using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.Models
{
    public class Trainer : ApplicationUser // inherited from the ApplicationUser Class
    {

        public List<TrainerSchedule> trainerSchedules { get; set; }
        
        public List<TrainingSession> trainerTrainingSessions { get; set; }

        // base class constructor parameters. child inherits everything from the parent class.
        public Trainer(string FirstName, string LastName, string SportType, string Email, string PhoneNumber, string Password) : base(FirstName, LastName, Email, PhoneNumber, Password) // added constructor so we can see if trainer role shows in db
        {
            this.sportType = SportType;
            this.trainerSchedules = new List<TrainerSchedule>();
            this.trainerTrainingSessions = new List<TrainingSession>();
            
        }
        public Trainer()
        {

        }
    }// end of class
}// end of namespace


