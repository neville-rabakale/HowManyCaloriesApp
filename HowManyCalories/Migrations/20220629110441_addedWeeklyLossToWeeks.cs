using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowManyCalories.Migrations
{
    public partial class addedWeeklyLossToWeeks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "WeeklyLoss",
                table: "Weeks",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WeeklyLoss",
                table: "Weeks");
        }
    }
}
