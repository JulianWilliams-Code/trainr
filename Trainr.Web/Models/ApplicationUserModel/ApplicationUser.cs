using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Trainr.Web.Models
{
    public class ApplicationUser : IdentityUser // since the user role is in the database as aspnetuser, using the identityuser library allows us to access the fileds needed such as the id an user name
    {

        // set property according to class Diagram

        // Required fields give you not null values
        public string firstName { get; set; }
       
        public string lastName { get; set; }

        public string fullName { get { return (firstName + " " + lastName); } } // provides fullname as an output

        // that is a null value because we also have an admin class that does not require everything in the user class.
        public string location { get; set; }

        public string sportType { get; set; } // put a null value so we can either put a value for the athlete or trainer or leave it empty for the admin

        // need credit card information for athlete and trainer.
        // we can using stripe which is a third party that gives you the credit card UI to give the athlete and trainer a way to make and recieve payments. 




        public ApplicationUser(string FirstName, string LastName, string email, string PhoneNumber, string Password)
        {
            this.firstName = FirstName;
            this.lastName = LastName;
            this.Email = email;
            this.UserName = email;
            this.PhoneNumber = PhoneNumber;
            this.location = null;
            this.sportType = null;
            PasswordHasher<ApplicationUser> passwordhasher = new PasswordHasher<ApplicationUser>();
            this.PasswordHash = passwordhasher.HashPassword(this, Password); // this represents the user roles
            this.SecurityStamp = Guid.NewGuid().ToString();

        }

        public ApplicationUser() // you have to create your default constructor
        {

        }// end of overloaded constructor

     

    }// end of class
}// end of namespace
