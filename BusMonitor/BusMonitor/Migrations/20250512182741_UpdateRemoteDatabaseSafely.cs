using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusMonitor.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRemoteDatabaseSafely : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Check if ArrivalTime column exists before adding it
            migrationBuilder.Sql(
                @"IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'ArrivalTime'
                    AND Object_ID = Object_ID(N'Trips')
                )
                BEGIN
                    ALTER TABLE [Trips] ADD [ArrivalTime] datetime2 NULL;
                END");

            // Check if DepartureTime column exists before adding it
            migrationBuilder.Sql(
                @"IF NOT EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'DepartureTime'
                    AND Object_ID = Object_ID(N'Trips')
                )
                BEGIN
                    ALTER TABLE [Trips] ADD [DepartureTime] datetime2 NULL;
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Only drop columns if they exist
            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'ArrivalTime'
                    AND Object_ID = Object_ID(N'Trips')
                )
                BEGIN
                    ALTER TABLE [Trips] DROP COLUMN [ArrivalTime];
                END");

            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1 FROM sys.columns 
                    WHERE Name = N'DepartureTime'
                    AND Object_ID = Object_ID(N'Trips')
                )
                BEGIN
                    ALTER TABLE [Trips] DROP COLUMN [DepartureTime];
                END");
        }
    }
}
