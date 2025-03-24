using Microsoft.EntityFrameworkCore;

namespace BusMonitor.Models
{
    public class BusMonitorDbContext : DbContext
    {
        public BusMonitorDbContext(DbContextOptions<BusMonitorDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<StudentTrip> StudentTrips { get; set; }
        public DbSet<StudentReport> StudentReports { get; set; }
        public DbSet<DelayReport> DelayReports { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure enum to string conversions
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<Trip>()
                .Property(t => t.Status)
                .HasConversion<string>();

            // Configure relationships

            // User as Parent to Student (one-to-many)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Parent)
                .WithMany(u => u.Students)
                .HasForeignKey(s => s.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // User as Driver to Trip (one-to-many)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(u => u.DrivenTrips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // User as Supervisor to Trip (one-to-many)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Supervisor)
                .WithMany(u => u.SupervisedTrips)
                .HasForeignKey(t => t.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // User as Admin to Trip (one-to-many)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Admin)
                .WithMany(u => u.CreatedTrips)
                .HasForeignKey(t => t.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Bus to Trip (one-to-many)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Bus)
                .WithMany(b => b.Trips)
                .HasForeignKey(t => t.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Route to Trip (one-to-many)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Route)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RouteId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentTrip (many-to-many relationship between Student and Trip)
            modelBuilder.Entity<StudentTrip>()
                .HasOne(st => st.Student)
                .WithMany(s => s.StudentTrips)
                .HasForeignKey(st => st.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentTrip>()
                .HasOne(st => st.Trip)
                .WithMany(t => t.StudentTrips)
                .HasForeignKey(st => st.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            // StudentReport relationships
            modelBuilder.Entity<StudentReport>()
                .HasOne(r => r.Student)
                .WithMany(s => s.BehaviorReports)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentReport>()
                .HasOne(r => r.Trip)
                .WithMany(t => t.BehaviorReports)
                .HasForeignKey(r => r.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentReport>()
                .HasOne(r => r.Supervisor)
                .WithMany(u => u.BehaviorReports)
                .HasForeignKey(r => r.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // DelayReport relationships
            modelBuilder.Entity<DelayReport>()
                .HasOne(r => r.Trip)
                .WithMany(t => t.DelayReports)
                .HasForeignKey(r => r.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DelayReport>()
                .HasOne(r => r.Supervisor)
                .WithMany(u => u.DelayReports)
                .HasForeignKey(r => r.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification relationships
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Parent)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Student)
                .WithMany(s => s.Notifications)
                .HasForeignKey(n => n.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.Trip)
                .WithMany(t => t.Notifications)
                .HasForeignKey(n => n.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add unique constraint to ensure a supervisor can only be assigned to one active trip at a time
            modelBuilder.Entity<Trip>()
                .HasIndex(t => new { t.SupervisorId, t.Status })
                .IsUnique()
                .HasFilter("[Status] = 'Active'");
        }
    }
}
