using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using BusMonitor.Models;
using BusMonitor.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Reflection;

namespace BusMonitorTest
{
    public class TripCreationServiceTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<ILogger<TripCreationService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BusMonitorDbContext _context;
        private readonly Mock<IServiceScope> _mockScope;
        private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
        
        private readonly List<User> _users;
        private readonly List<Bus> _buses;
        private readonly List<Route> _routes;
        private readonly List<Trip> _trips;

        public TripCreationServiceTests()
        {
            // Setup in-memory data
            _users = new List<User>
            {
                new User { Id = 1, Name = "Admin User", Username = "admin", Password = "adminpass", PhoneNumber = "111-111-1111", Role = Role.Admin },
                new User { Id = 2, Name = "Supervisor User", Username = "supervisor1", Password = "superpass", PhoneNumber = "222-222-2222", Role = Role.Supervisor },
                new User { Id = 3, Name = "Driver User", Username = "driver1", Password = "driverpass", PhoneNumber = "333-333-3333", Role = Role.Driver }
            };

            _buses = new List<Bus>
            {
                new Bus { Id = 1, Number = "B001", Capacity = 50 }
            };

            _routes = new List<Route>
            {
                new Route { Id = 1, StartPoint = "School", EndPoint = "Downtown", Stops = "Stop 1, Stop 2" }
            };

            _trips = new List<Trip>();

            // Setup in-memory database
            var options = new DbContextOptionsBuilder<BusMonitorDbContext>()
                .UseInMemoryDatabase(databaseName: $"BusMonitorTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new BusMonitorDbContext(options);
            
            // Add test data to in-memory database
            _context.Users.AddRange(_users);
            _context.Buses.AddRange(_buses);
            _context.Routes.AddRange(_routes);
            _context.SaveChanges();

            // Setup mock service provider
            _mockScope = new Mock<IServiceScope>();
            _mockScope.Setup(s => s.ServiceProvider.GetService(typeof(BusMonitorDbContext)))
                     .Returns(_context);

            _mockScopeFactory = new Mock<IServiceScopeFactory>();
            _mockScopeFactory.Setup(f => f.CreateScope()).Returns(_mockScope.Object);

            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockServiceProvider.Setup(s => s.GetService(typeof(IServiceScopeFactory)))
                              .Returns(_mockScopeFactory.Object);

            // Setup mock logger and mapper
            _mockLogger = new Mock<ILogger<TripCreationService>>();
            _mockMapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task CreateDailyTripsAsync_ShouldCreateTrips()
        {
            // Arrange
            var tripCreationService = new TripCreationService(
                _mockServiceProvider.Object,
                _mockLogger.Object,
                _mockMapper.Object);

            // Act - Call the private method using reflection
            MethodInfo methodInfo = typeof(TripCreationService).GetMethod("CreateDailyTripsAsync", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            
            var task = (Task)methodInfo.Invoke(tripCreationService, null);
            await task;

            // Assert
            var trips = _context.Trips.ToList();
            Assert.NotEmpty(trips);
            Assert.Single(trips); // 1 supervisor * 1 driver * 1 bus * 1 route = 1 trip
            
            var trip = trips.First();
            Assert.Equal(Status.Planned, trip.Status);
            Assert.Equal(1, trip.BusId);
            Assert.Equal(1, trip.RouteId);
            Assert.Equal(3, trip.DriverId);
            Assert.Equal(2, trip.SupervisorId);
            Assert.Equal(1, trip.AdminId);
        }
    }
} 