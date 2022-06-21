using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HowManyCalories.Data.Migrations
{
    public partial class AddedDurationToUserProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Duration",
                table: "UserProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "UserProfiles");
        }
    }
}
