using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusMonitor.Migrations
{
    /// <inheritdoc />
    public partial class CapturePendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DepartureTime",
                table: "Trips",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArrivalTime",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "DepartureTime",
                table: "Trips");
        }
    }
}
