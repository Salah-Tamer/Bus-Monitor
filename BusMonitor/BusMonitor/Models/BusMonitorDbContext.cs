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
        }
    }
}
