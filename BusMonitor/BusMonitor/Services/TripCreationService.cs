using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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


                                    // Create new trip
                                    var tripDto = new TripDTO
                                    {
                                        Status = "Planned",
                                        BusId = bus.Id,
                                        RouteId = route.Id,
                                        DriverId = driver.Id,
                                        SupervisorId = supervisor.Id,
                                        AdminId = 1,
                                        ArrivalTime = DateTime.UtcNow.AddHours(1),
                                        DepartureTime = DateTime.UtcNow
                                    };

                                    context.Trips.Add(_mapper.Map<Trip>(tripDto));
                                }
                            }
                        }
                    }

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Daily trips created successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while creating daily trips.");
                }
            }
        }
    }
}

