using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusMonitor.Migrations
{
    /// <inheritdoc />
    public partial class AddBehaviorCategoryToStudentReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BehaviorCategory",
                table: "StudentReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BehaviorCategory",
                table: "StudentReports");
        }
    }
}
