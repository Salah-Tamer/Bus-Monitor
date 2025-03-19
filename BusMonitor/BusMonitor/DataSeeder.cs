using BusMonitor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Route = BusMonitor.Models.Route;

namespace BusMonitor.Data
{
    public static class DataSeeder
    {
        public static void SeedData(BusMonitorDbContext context)
        {
            // Only seed if the database is empty
            if (context.Users.Any())
                return;

            // Seed Users
            var users = new List<User>
            {
                // Admins
                new User { Name = "Amr Mahmoud", Username = "amr.admin", Password = "P@ssw0rd", PhoneNumber = "0100123456", Role = Role.Admin },
                new User { Name = "Fatma Ibrahim", Username = "fatma.admin", Password = "P@ssw0rd", PhoneNumber = "0111234567", Role = Role.Admin },
                
                // Drivers
                new User { Name = "Hassan Ali", Username = "hassan.driver", Password = "P@ssw0rd", PhoneNumber = "0122345678", Role = Role.Driver },
                new User { Name = "Karim Salah", Username = "karim.driver", Password = "P@ssw0rd", PhoneNumber = "0133456789", Role = Role.Driver },
                new User { Name = "Mahmoud Samir", Username = "mahmoud.driver", Password = "P@ssw0rd", PhoneNumber = "0144567890", Role = Role.Driver },
                
                // Supervisors
                new User { Name = "Nour Ahmed", Username = "nour.supervisor", Password = "P@ssw0rd", PhoneNumber = "0155678901", Role = Role.Supervisor },
                new User { Name = "Omar Khaled", Username = "omar.supervisor", Password = "P@ssw0rd", PhoneNumber = "0166789012", Role = Role.Supervisor },
                
                // Parents
                new User { Name = "Rania Mohamed", Username = "rania.parent", Password = "P@ssw0rd", PhoneNumber = "0177890123", Role = Role.Parent },
                new User { Name = "Sameh Adel", Username = "sameh.parent", Password = "P@ssw0rd", PhoneNumber = "0188901234", Role = Role.Parent },
                new User { Name = "Tamer Hossam", Username = "tamer.parent", Password = "P@ssw0rd", PhoneNumber = "0199012345", Role = Role.Parent },
                new User { Name = "Walaa Mostafa", Username = "walaa.parent", Password = "P@ssw0rd", PhoneNumber = "0120123456", Role = Role.Parent },
                new User { Name = "Yasser Gamal", Username = "yasser.parent", Password = "P@ssw0rd", PhoneNumber = "0130234567", Role = Role.Parent }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // Seed Students
            var students = new List<Student>
            {
                new Student { Name = "Ahmed Rania", Grade = "Grade 5", Absence = 0, ParentId = users.Single(u => u.Username == "rania.parent").Id },
                new Student { Name = "Mariam Rania", Grade = "Grade 3", Absence = 1, ParentId = users.Single(u => u.Username == "rania.parent").Id },
                new Student { Name = "Yousef Sameh", Grade = "Grade 6", Absence = 2, ParentId = users.Single(u => u.Username == "sameh.parent").Id },
                new Student { Name = "Laila Tamer", Grade = "Grade 4", Absence = 0, ParentId = users.Single(u => u.Username == "tamer.parent").Id },
                new Student { Name = "Ziad Tamer", Grade = "Grade 2", Absence = 3, ParentId = users.Single(u => u.Username == "tamer.parent").Id },
                new Student { Name = "Malak Walaa", Grade = "Grade 1", Absence = 0, ParentId = users.Single(u => u.Username == "walaa.parent").Id },
                new Student { Name = "Kareem Yasser", Grade = "Grade 5", Absence = 1, ParentId = users.Single(u => u.Username == "yasser.parent").Id },
                new Student { Name = "Nada Yasser", Grade = "Grade 3", Absence = 0, ParentId = users.Single(u => u.Username == "yasser.parent").Id }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            // Seed Buses
            var buses = new List<Bus>
            {
                new Bus { Number = "CAI-1001", Capacity = 30 },
                new Bus { Number = "CAI-1002", Capacity = 25 },
                new Bus { Number = "CAI-1003", Capacity = 30 },
                new Bus { Number = "CAI-1004", Capacity = 20 }
            };

            context.Buses.AddRange(buses);
            context.SaveChanges();

            // Seed Routes
            var routes = new List<Route>
            {
                new Route {
                    StartPoint = "Cairo International School",
                    EndPoint = "Maadi",
                    Stops = "Zamalek, Garden City, Old Cairo"
                },
                new Route {
                    StartPoint = "Cairo International School",
                    EndPoint = "Heliopolis",
                    Stops = "Nasr City, Sheraton, Almaza"
                },
                new Route {
                    StartPoint = "Cairo International School",
                    EndPoint = "6th of October",
                    Stops = "Sheikh Zayed, Smart Village, Hyper One"
                },
                new Route {
                    StartPoint = "Cairo International School",
                    EndPoint = "New Cairo",
                    Stops = "5th Settlement, American University, Rehab City"
                }
            };

            context.Routes.AddRange(routes);
            context.SaveChanges();

            // Seed Trips
            var adminId = users.First(u => u.Role == Role.Admin).Id;
            var driverIds = users.Where(u => u.Role == Role.Driver).Select(u => u.Id).ToList();
            var supervisorIds = users.Where(u => u.Role == Role.Supervisor).Select(u => u.Id).ToList();

            var trips = new List<Trip>
            {
                new Trip {
                    Status = Status.Active,
                    BusId = buses[0].Id,
                    RouteId = routes[0].Id,
                    DriverId = driverIds[0],
                    SupervisorId = supervisorIds[0],
                    AdminId = adminId
                },
                new Trip {
                    Status = Status.Planned,
                    BusId = buses[1].Id,
                    RouteId = routes[1].Id,
                    DriverId = driverIds[1],
                    SupervisorId = supervisorIds[1],
                    AdminId = adminId
                },
                new Trip {
                    Status = Status.Completed,
                    BusId = buses[2].Id,
                    RouteId = routes[2].Id,
                    DriverId = driverIds[2],
                    SupervisorId = supervisorIds[0],
                    AdminId = adminId
                },
                new Trip {
                    Status = Status.Planned,
                    BusId = buses[3].Id,
                    RouteId = routes[3].Id,
                    DriverId = driverIds[0],
                    SupervisorId = supervisorIds[1],
                    AdminId = adminId
                }
            };

            context.Trips.AddRange(trips);
            context.SaveChanges();

            // Seed StudentTrips
            var studentTrips = new List<StudentTrip>();

            // Assign students to the Maadi route
            studentTrips.AddRange(new List<StudentTrip>
            {
                new StudentTrip { StudentId = students[0].Id, TripId = trips[0].Id, },
                new StudentTrip { StudentId = students[1].Id, TripId = trips[0].Id, }
            });

            // Assign students to the Heliopolis route
            studentTrips.AddRange(new List<StudentTrip>
            {
                new StudentTrip { StudentId = students[2].Id, TripId = trips[1].Id },
                new StudentTrip { StudentId = students[3].Id, TripId = trips[1].Id }
            });

            // Assign students to the 6th of October route
            studentTrips.AddRange(new List<StudentTrip>
            {
                new StudentTrip { StudentId = students[4].Id, TripId = trips[2].Id },
                new StudentTrip { StudentId = students[5].Id, TripId = trips[2].Id }
            });

            // Assign students to the New Cairo route
            studentTrips.AddRange(new List<StudentTrip>
            {
                new StudentTrip { StudentId = students[6].Id, TripId = trips[3].Id },
                new StudentTrip { StudentId = students[7].Id, TripId = trips[3].Id }
            });

            context.StudentTrips.AddRange(studentTrips);
            context.SaveChanges();
        }
    }
}
