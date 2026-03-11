using System;
using System.Collections.Generic;
using System.Text;
using Trainr.Web.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Trainr.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // dbset is the connection manager that allows you set tables in SQL from C#
        public DbSet<Athlete> Athletes { get; set; }

        public DbSet<Trainer> Trainers { get; set; }

        public DbSet<TrainingSession> TrainingSessions { get; set; }

        public DbSet<TrainerSchedule> TrainerSchedules { get; set; }



        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}
