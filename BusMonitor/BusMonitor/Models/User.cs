﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusMonitor.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(20)]
        public Role Role { get; set; } // Admin, Driver, Supervisor, Parent

        // Navigation properties
        public virtual ICollection<Student> Students { get; set; } // For Parent role
        public virtual ICollection<Trip> SupervisedTrips { get; set; } // For Supervisor role
        public virtual ICollection<Trip> DrivenTrips { get; set; } // For Driver role
        public virtual ICollection<Trip> CreatedTrips { get; set; } // For Admin role
        public virtual ICollection<StudentReport> BehaviorReports { get; set; }
        public virtual ICollection<DelayReport> DelayReports { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public User()
        {
            Students = new HashSet<Student>();
            SupervisedTrips = new HashSet<Trip>();
            DrivenTrips = new HashSet<Trip>();
            CreatedTrips = new HashSet<Trip>();
        }

        // These methods are for convenience but should prefer using the injected PasswordHasher service
        // directly in services and controllers when possible
        public bool ValidatePassword(string password)
        {
            var passwordHasher = new PasswordHasher();
            return passwordHasher.VerifyHashedPassword(this.Password, password);
        }

        public void SetPassword(string password)
        {
            var passwordHasher = new PasswordHasher();
            Password = passwordHasher.HashPassword(password);
        }
    }
}
