using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BusMonitor.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using BusMonitor.DTOs;

namespace BusMonitor.Services
{
    public class TripCreationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TripCreationService> _logger;
        private readonly IMapper _mapper;

        public TripCreationService(IServiceProvider serviceProvider, ILogger<TripCreationService> logger, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _mapper = mapper;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CreateDailyTripsAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken); // Run once every day
            }
        }

        private async Task CreateDailyTripsAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<BusMonitorDbContext>();

                try
                {
                    // Fetch all required data
                    var supervisors = await context.Users.Where(u => u.Role == Role.Supervisor).ToListAsync();
                    var drivers = await context.Users.Where(u => u.Role == Role.Driver).ToListAsync();
                    var buses = await context.Buses.ToListAsync();
                    var routes = await context.Routes.ToListAsync();

                    if (!supervisors.Any() || !drivers.Any() || !buses.Any() || !routes.Any())
                    {
                        _logger.LogWarning("Insufficient data to create trips.");
                        return;
                    }

                    // Pre-fetch existing trips for today
                    var existingTrips = await context.Trips
                        .Where(t => t.DepartureTime.HasValue && t.DepartureTime.Value.Date == DateTime.UtcNow.Date)
                        .Select(t => new { t.BusId, t.RouteId, t.DriverId, t.SupervisorId })
                        .ToListAsync();

                    /*
                    * ORIGINAL CODE (REFACTORED):
                    * This implementation used deeply nested loops which made the code harder to read and maintain.
                    * The refactored version below extracts this logic into a separate method for better organization.
                    *
                    int duplicatesSkipped = 0;

                    foreach (var supervisor in supervisors)
                    {
                        foreach (var driver in drivers)
                        {
                            foreach (var bus in buses)
                            {
                                foreach (var route in routes)
                                {
                                    // Check against in-memory list
                                    bool isDuplicate = existingTrips.Any(t =>
                                        t.BusId == bus.Id &&
                                        t.RouteId == route.Id &&
                                        t.DriverId == driver.Id &&
                                        t.SupervisorId == supervisor.Id);

                                    if (isDuplicate)
                                    {
                                        duplicatesSkipped++;
                                        continue;
                                    }

                                    // Create new trip directly as entity instead of using mapper
                                    var trip = new Trip
                                    {
                                        Status = Status.Planned,
                                        BusId = bus.Id,
                                        RouteId = route.Id,
                                        DriverId = driver.Id,
                                        SupervisorId = supervisor.Id,
                                        AdminId = 1,
                                        ArrivalTime = DateTime.UtcNow.AddHours(1),
                                        DepartureTime = DateTime.UtcNow
                                    };
                                    
                                    context.Trips.Add(trip);
                                }
                            }
                        }
                    }
                    */

                    // Refactored approach using combinations
                    var tripsToCreate = GenerateTripCombinations(supervisors, drivers, buses, routes, existingTrips);
                    
                    foreach (var trip in tripsToCreate)
                    {
                        context.Trips.Add(trip);
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation($"Daily trips created successfully. Created {tripsToCreate.Count} trips.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating daily trips.");
                }
            }
        }

        private List<Trip> GenerateTripCombinations(
            List<User> supervisors, 
            List<User> drivers, 
            List<Bus> buses, 
            List<Route> routes,
            IEnumerable<dynamic> existingTrips)
        {
            var tripsToCreate = new List<Trip>();
            int duplicatesSkipped = 0;

            // Generate all possible combinations
            var combinations = from supervisor in supervisors
                               from driver in drivers
                               from bus in buses
                               from route in routes
                               select new { supervisor, driver, bus, route };

            foreach (var combo in combinations)
            {
                // Check if this combination already exists
                bool isDuplicate = existingTrips.Any(t =>
                    t.BusId == combo.bus.Id &&
                    t.RouteId == combo.route.Id &&
                    t.DriverId == combo.driver.Id &&
                    t.SupervisorId == combo.supervisor.Id);

                if (isDuplicate)
                {
                    duplicatesSkipped++;
                    continue;
                }

                // Create new trip
                var trip = new Trip
                {
                    Status = Status.Planned,
                    BusId = combo.bus.Id,
                    RouteId = combo.route.Id,
                    DriverId = combo.driver.Id,
                    SupervisorId = combo.supervisor.Id,
                    AdminId = 1,
                    ArrivalTime = DateTime.UtcNow.AddHours(1),
                    DepartureTime = DateTime.UtcNow
                };

                tripsToCreate.Add(trip);
            }

            _logger.LogInformation($"Generated {tripsToCreate.Count} trips. Skipped {duplicatesSkipped} duplicates.");
            return tripsToCreate;
        }
    }
}

