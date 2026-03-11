using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Trainr.Web.Data.Migrations
{
    public partial class AddedApplicationUsers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "firstName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lastName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "location",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sportType",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "athletePosition",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "TrainerSchedules",
                columns: table => new
                {
                    trainerScheduleID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainerScheduleStartDateTime = table.Column<DateTime>(nullable: false),
                    trainerScheduleEndDateTime = table.Column<DateTime>(nullable: false),
                    isAvailable = table.Column<bool>(nullable: false),
                    trainerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainerSchedules", x => x.trainerScheduleID);
                    table.ForeignKey(
                        name: "FK_TrainerSchedules_AspNetUsers_trainerId",
                        column: x => x.trainerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    trainingSessionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    trainingSessionStartTime = table.Column<DateTime>(nullable: false),
                    trainingSessionEndTime = table.Column<DateTime>(nullable: false),
                    athleteId = table.Column<string>(nullable: true),
                    trainerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.trainingSessionID);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_AspNetUsers_athleteId",
                        column: x => x.athleteId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrainingSessions_AspNetUsers_trainerId",
                        column: x => x.trainerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrainerSchedules_trainerId",
                table: "TrainerSchedules",
                column: "trainerId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_athleteId",
                table: "TrainingSessions",
                column: "athleteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingSessions_trainerId",
                table: "TrainingSessions",
                column: "trainerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrainerSchedules");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropColumn(
                name: "firstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "lastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "location",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "sportType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "athletePosition",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
