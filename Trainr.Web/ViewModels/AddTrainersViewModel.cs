using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.ViewModels
{
    public class AddTrainersViewModel
    {
        [Required(ErrorMessage = "Must enter trainer first name")]
        [StringLength(maximumLength:50, MinimumLength = 1, ErrorMessage = "Please enter a least one character" )]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Must enter trainer last name")]
        [StringLength(maximumLength: 50, MinimumLength = 1, ErrorMessage = "Please enter a least one character")]
        public string lastName { get; set; }
        [Required]
        public string sportType { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string phoneNumber { get; set; }
        [Required]
        public string passWord { get; set; }
        [Required]
        public DateTime StartDateTime { get; set; }
        [Required]
        public DateTime EndDateTime { get; set; }

    }
}
