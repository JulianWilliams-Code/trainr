using System;
using Xunit;
using Moq;
using Trainr.Web.Models;
using Trainr.Web.Controllers;
using System.Collections.Generic;
using Trainr.Web.ViewModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Trainr.Web.Models.TrainingSessionModel;

namespace Trainr.Tests
{
    public class TrainerTest
    {
        //Attributes

        private Mock<ITrainerRepo> mockTrainerRepo;
        private Mock<ITrainerScheduleRepo> mockTrainerScheduleRepo;
        private Mock<IApplicationUserRepo> mockApplicationUserRepo;
        private Mock<ITrainingSessionRepo> mockTrainingSessionRepo;
        private TrainersController trainersController;
        private TrainerScheduleController trainerScheduleController;
        private TrainingSessionController trainingSessionController;

        private List<Trainer> mockTrainerList;
        private List<TrainerSchedule> mockTrainerScheduleList;
        

        public TrainerTest()
        {
            mockTrainerRepo = new Mock<ITrainerRepo>();
            mockTrainerScheduleRepo = new Mock<ITrainerScheduleRepo>();
            mockApplicationUserRepo = new Mock<IApplicationUserRepo>();
            mockTrainingSessionRepo = new Mock<ITrainingSessionRepo>();

            
            trainersController = new TrainersController(mockTrainerRepo.Object); //you must use .Object otherwise it will not bring in mockTrainerRepo
            trainerScheduleController = new TrainerScheduleController(mockTrainerRepo.Object, mockTrainerScheduleRepo.Object, mockApplicationUserRepo.Object,mockTrainingSessionRepo.Object);
            trainingSessionController = new TrainingSessionController(mockTrainerRepo.Object, mockTrainerScheduleRepo.Object, mockApplicationUserRepo.Object, mockTrainingSessionRepo.Object);

            mockTrainerList = new List<Trainer>(); // if you dont have a list, you will NOT BE ABLE to add individual trainer objects to a list VITAL!
            mockTrainerScheduleList = new List<TrainerSchedule>();
        }
        [Fact] //decorater for method
        public void ShouldListAllAvailableTrainers() //name test based on what it should be doing
        {
            //Here we will test ListAllTrainersHelper controller method
            mockTrainerList = MockTrainerData(); //this is how you will get the data
            mockTrainerRepo.Setup(m => m.ListAllAvailableTrainers()).Returns(mockTrainerList); //MUST DO THIS

            int expectedNumberOfTrainers = mockTrainerList.Count;
            int actualNumberOfTrainers = 0;

            //2. Act
            List<Trainer> actualTrainerList = trainersController.ListAllAvailableTrainersHelper();
            actualNumberOfTrainers = actualTrainerList.Count;
            //3. Assert
            Assert.Equal(expectedNumberOfTrainers, actualNumberOfTrainers);
            Assert.Equal(mockTrainerList, actualTrainerList);
        }

        [Fact]
        public void ShouldSearchForTrainers()
        {
            mockTrainerScheduleList = MockTrainerScheduleData(); 
            mockTrainerScheduleRepo.Setup(m => m.ListOfTrainerSchedules()).Returns(mockTrainerScheduleList); 

            int expectedNumberOfTrainers = mockTrainerScheduleList.Count;
            int actualNumberOfTrainers = 0;
            SearchTrainersViewModel trainerViewModel = new SearchTrainersViewModel();
          
            trainerViewModel.SportType = null;
            trainerViewModel.StartDateTime = null;
            trainerViewModel.EndDateTime = null;
            //2. Act
            List<TrainerSchedule> actualTrainerScheduleList = trainerScheduleController.SearchTrainersHelper(trainerViewModel);
            actualNumberOfTrainers = actualTrainerScheduleList.Count;
            //3. Assert
            Assert.Equal(expectedNumberOfTrainers, actualNumberOfTrainers);
            Assert.Equal(mockTrainerScheduleList, actualTrainerScheduleList);
        }

        [Fact]
        public void ShouldSearchForFootballTrainers()
        {
            mockTrainerScheduleList = MockTrainerScheduleData();
            mockTrainerScheduleRepo.Setup(m => m.ListOfTrainerSchedules()).Returns(mockTrainerScheduleList);

            int expectedNumberOfTrainers;
            int actualNumberOfTrainers = 0;
            SearchTrainersViewModel trainerViewModel = new SearchTrainersViewModel();
           
            trainerViewModel.SportType = "Football";
            trainerViewModel.StartDateTime = null;
            trainerViewModel.EndDateTime = null;
            //2. Act
            List<TrainerSchedule> expectedTrainerList = mockTrainerScheduleList
                .Where(ts => ts.trainer.sportType == trainerViewModel.SportType
                    ).ToList<TrainerSchedule>();
            expectedNumberOfTrainers = expectedTrainerList.Count;
            List<TrainerSchedule> actualTrainerScheduleList = trainerScheduleController.SearchTrainersHelper(trainerViewModel);
            actualNumberOfTrainers = actualTrainerScheduleList.Count;
            //3. Assert
            Assert.Equal(expectedNumberOfTrainers, actualNumberOfTrainers);
            Assert.Equal(expectedTrainerList, actualTrainerScheduleList);
        }

        [Fact]
        public void ShouldAddTrainer()
        {
            //Arrange

            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";
            Trainer expectedTrainer = trainer;
            Trainer actualAddedTrainer = null;
            mockTrainerScheduleRepo.Setup(m => m.AddTrainerAsync(It.IsAny<Trainer>())).Returns(Task.CompletedTask).Callback<Trainer>(t => actualAddedTrainer = t);
            mockTrainerScheduleRepo
                .Setup(m => m.AddTrainerAsync(It.IsAny<Trainer>()))
                .Returns(Task.CompletedTask)
                .Callback<Trainer>(t => actualAddedTrainer = t);
            //Act
            trainerScheduleController.AddTrainerHelper(trainer).Wait();
            //Assert
            Assert.Equal(expectedTrainer.fullName, actualAddedTrainer.fullName);
            //Assert.Equal(expectedTrainer,actualAddedTrainer);

        }

        [Fact]
        public void ShouldAddTrainerSchedule()
        {
            // Arrange
            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";

            DateTime scheduleStartDate = new DateTime(2019, 8, 1, 1, 30, 00);
            DateTime scheduleEndDate = new DateTime(2019, 8, 1, 2, 00, 00);
            TrainerSchedule trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 1; 
            trainerSchedule.trainer = trainer; // oop connection between trainer and trainerSchedule

            TrainerSchedule expectedTrainerSchedule = trainerSchedule;
            TrainerSchedule actualAddedTrainerSchedule = null;


            mockTrainerScheduleRepo.Setup(m => m.AddTrainerScheduleAsync(It.IsAny<TrainerSchedule>())).Returns(Task.CompletedTask).Callback<TrainerSchedule>(ts => actualAddedTrainerSchedule = ts);


            // Act
            trainerScheduleController.AddTrainerScheduleHelper(trainerSchedule).Wait();


            // Assert 
            Assert.Equal(expectedTrainerSchedule, actualAddedTrainerSchedule);

        }

        [Fact]
        public void ShouldEditTrainer()
        {
            //1. Arrange
            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";

            mockTrainerRepo.Setup(m => m.EditTrainerAsync(It.IsAny<Trainer>()));

            //2. Act

            IActionResult result = trainerScheduleController.EditTrainer(trainer);

            //3. Assert

            //Only thing we can check for is if this method was called

            mockTrainerRepo.Verify
                (m => m.EditTrainerAsync(It.IsAny<Trainer>()), Times.Once);

            //What if we delete multiple players at the same time? Should we still use Times.Once?

        }

        [Fact]
        public void ShouldEditTrainerSchedule()
        {
            //1. Arrange
            TrainerSchedule trainerSchedule = new TrainerSchedule(); /*[2019,8,1,1,30,00], [2019, 8, 1, 2, 00, 00], "1");*/
            trainerSchedule.trainerScheduleID = 1;

            mockTrainerScheduleRepo.Setup(m => m.EditTrainerScheduleAsync(It.IsAny<TrainerSchedule>()));

            //2. Act

            IActionResult result = trainerScheduleController.EditTrainerSchedule(trainerSchedule);

            //3. Assert

            //Only thing we can check for is if this method was called

            mockTrainerScheduleRepo.Verify
                (m => m.EditTrainerScheduleAsync(It.IsAny<TrainerSchedule>()), Times.Once);

            //What if we delete multiple players at the same time? Should we still use Times.Once?

        }

        [Fact]
        public void ShouldDeleteTrainer()
        {
            //1. Arrange
            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";

            mockTrainerRepo.Setup(m => m.DeleteTrainerAsync(It.IsAny<Trainer>()));

            //2. Act
            IActionResult result = trainerScheduleController.DeleteTrainer(trainer);

            //3. Assert
            mockTrainerRepo.Verify
                (m => m.DeleteTrainerAsync(It.IsAny<Trainer>()), Times.Once);
        }

        [Fact]
        public void ShouldDeleteTrainerSchedule()
        {
            //1. Arrange
            TrainerSchedule trainerSchedule = new TrainerSchedule(); /*[2019,8,1,1,30,00], [2019, 8, 1, 2, 00, 00], "1");*/
            trainerSchedule.trainerScheduleID = 1;

            mockTrainerScheduleRepo.Setup(m => m.DeleteTrainerScheduleAsync(It.IsAny<TrainerSchedule>()));

            //2. Act
            IActionResult result = trainerScheduleController.DeleteTrainerSchedule(trainerSchedule);

            //3. Assert
            mockTrainerScheduleRepo.Verify
                (m => m.DeleteTrainerScheduleAsync(It.IsAny<TrainerSchedule>()), Times.Once);
        }

        [Fact]

        public void ShouldBookTrainingSession()
        {
            string athleteID = "S001";
            DateTime requestedTrainingSessionDate =
                DateTime.Parse("4/2/2020 2:00 PM");
            DateTime requestedTrainingSessionEndDate =
                DateTime.Parse("4/3/2020 2:00 PM");
            
            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");

            TrainerSchedule trainerSchedule = new TrainerSchedule();

            

            mockTrainerScheduleRepo.Setup
                (m => m.FindTrainerSchedule
                (It.IsAny<int>())).Returns(trainerSchedule);

      
            TrainingSession trainingSession = new TrainingSession(requestedTrainingSessionDate,requestedTrainingSessionEndDate,athleteID,trainer.Id);




            //Act
            //bool bookedTrainingSession = 


            //Assert
            //Assert.True(bookedTrainingSession);
        }

        [Fact]

        public void ShouldNotBookTrainingSession()
        {
            string athleteID = "S001";
            DateTime requestedTrainingSessionDate =
                DateTime.Parse("4/2/2020 2:00 PM");

            DateTime requestedTrainingSessionEndDate =
                DateTime.Parse("4/3/2020 2:00 PM");

            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");

            TrainerSchedule trainerSchedule = new TrainerSchedule();

            mockTrainerScheduleRepo.Setup
                (m => m.FindTrainerSchedule
                (It.IsAny<int>())).Returns(trainerSchedule);

            TrainingSession trainingSession = null;

            


            ////Act
            //bool bookedTrainingSession =
            //    trainingSessionController.BookTrainingSessionHelper(athleteID, requestedTrainingSessionDate,requestedTrainingSessionEndDate);

            ////Assert
            //Assert.False(bookedTrainingSession);
        }




        public List<Trainer> MockTrainerData()
        {
            
            List<Trainer> trainerList = new List<Trainer>();

            
            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";
            trainerList.Add(trainer);
            

            trainer = new Trainer("Nanda", "Surendra", "Soccer", "NS1111@gmail.com", "(104)-333-3333", "Trainer2");
            trainer.Id = "2";           
            trainerList.Add(trainer);
            

            trainer = new Trainer("JJ", " Watt", "Basketball", "JW1111@gmail.com", "(101)-101-1010", "Trainer3");
            trainer.Id = "3";            
            trainerList.Add(trainer);
            

            trainer = new Trainer("Jesus", "Love", "Football", "JL1111@gmail.com", "(102)-222-2222", "Trainer1");
            trainer.Id = "4";          
            trainerList.Add(trainer);
       

            trainer = new Trainer("Salman", "Mann", "Soccer", "SM1111@gmail.com", "(103)-333-3333", "Trainer2");
            trainer.Id = "5";         
            trainerList.Add(trainer);
            

            trainer = new Trainer("TJ", "Jackson", "Basketball", "TJ1111@gmail.com", "(101)-101-1010", "Trainer3");
            trainer.Id = "6";
            trainerList.Add(trainer);
         
            return trainerList;
        }


        public List<TrainerSchedule> MockTrainerScheduleData()
        {
            List<TrainerSchedule> trainerSchedulesList = new List<TrainerSchedule>();


            Trainer trainer = new Trainer("Jorge", "Diaz", "Football", "Jd1111@gmail.com", "(555)-222-2222", "Trainer1");
            trainer.Id = "1";
            DateTime scheduleStartDate = new DateTime(2019, 8, 1, 1, 30, 00);
            DateTime scheduleEndDate = new DateTime(2019, 8, 1, 2, 00, 00);
            TrainerSchedule trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 1; // oop connection between trainer and trainerSchedule
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);


            trainer = new Trainer("Nanda", "Surendra", "Soccer", "NS1111@gmail.com", "(104)-333-3333", "Trainer2");
            trainer.Id = "2";
            scheduleStartDate = new DateTime(2019, 12, 12, 5, 00, 00);
            scheduleEndDate = new DateTime(2019, 12, 12, 5, 30, 00);
            trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 2;
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);


            trainer = new Trainer("JJ", " Watt", "Basketball", "JW1111@gmail.com", "(101)-101-1010", "Trainer3");
            trainer.Id = "3";
            scheduleStartDate = new DateTime(2019, 10, 8, 9, 30, 00);
            scheduleEndDate = new DateTime(2019, 10, 8, 10, 00, 00);
            trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 3;
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);


            trainer = new Trainer("Jesus", "Love", "Football", "JL1111@gmail.com", "(102)-222-2222", "Trainer1");
            trainer.Id = "4";
            scheduleStartDate = new DateTime(2019, 9, 1, 2, 00, 00);
            scheduleEndDate = new DateTime(2019, 9, 1, 2, 30, 00);
            trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 4;
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);


            trainer = new Trainer("Salman", "Mann", "Soccer", "SM1111@gmail.com", "(103)-333-3333", "Trainer2");
            trainer.Id = "5";
            scheduleStartDate = new DateTime(2019, 11, 12, 4, 00, 00);
            scheduleEndDate = new DateTime(2019, 11, 12, 4, 30, 00);
            trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 5;
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);


            trainer = new Trainer("TJ", "Jackson", "Basketball", "TJ1111@gmail.com", "(101)-101-1010", "Trainer3");
            trainer.Id = "6";
            scheduleStartDate = new DateTime(2019, 7, 8, 8, 30, 00);
            scheduleEndDate = new DateTime(2019, 7, 8, 9, 00, 00);
            trainerSchedule = new TrainerSchedule(scheduleStartDate, scheduleEndDate, trainer.Id);
            trainerSchedule.trainerScheduleID = 6;
            trainerSchedule.trainer = trainer;
            trainerSchedulesList.Add(trainerSchedule);

            return trainerSchedulesList;
        }
    }
}
