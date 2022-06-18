using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowManyCalories.Data.Migrations
{
    public partial class AddedUserProfileAndWeekToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartWeight = table.Column<double>(type: "float", nullable: false),
                    GoalWeight = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    WeekNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpectedWeight = table.Column<double>(type: "float", nullable: false),
                    AverageWeight = table.Column<double>(type: "float", nullable: false),
                    WeeklyCalories = table.Column<int>(type: "int", nullable: false),
                    CurrentCalories = table.Column<int>(type: "int", nullable: false),
                    CheckIn1 = table.Column<double>(type: "float", nullable: false),
                    CheckIn2 = table.Column<double>(type: "float", nullable: false),
                    CheckIn3 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => x.WeekNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Weeks");
        }
    }
}
