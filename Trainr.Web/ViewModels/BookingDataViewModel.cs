using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trainr.Web.ViewModels
{
    public class BookingDataViewModel
    {
        public string trainerFullName { get; set; }

        public string sportType { get; set; }

        public int totalBookedSessions { get; set; }
    }
}
