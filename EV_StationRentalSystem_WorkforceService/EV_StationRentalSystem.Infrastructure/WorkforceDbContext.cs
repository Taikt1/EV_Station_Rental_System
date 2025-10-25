using EV_StationRentalSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure
{
    public class WorkforceDbContext : DbContext
    {
        public WorkforceDbContext(DbContextOptions<WorkforceDbContext> options) : base(options) { }

        public DbSet<Workday> Workdays { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<StaffAssignment> StaffAssignments { get; set; }
        public DbSet<StaffReassignment> StaffReassignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Workday (1:N) Staff_Assignment
            modelBuilder.Entity<Workday>()
                .HasMany(w => w.StaffAssignments)
                .WithOne(a => a.Workday)
                .HasForeignKey(a => a.WorkdayId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shift (1:N) Staff_Assignment
            modelBuilder.Entity<Shift>()
                .HasMany(s => s.StaffAssignments)
                .WithOne(a => a.Shift)
                .HasForeignKey(a => a.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            // Shift (1:N) Staff_Assignment
            modelBuilder.Entity<Shift>()
                .HasMany(s => s.StaffReassignments)
                .WithOne(a => a.Shift)
                .HasForeignKey(a => a.ShiftId)
                .OnDelete(DeleteBehavior.Restrict);

            // Staff_Assignment (1:N) Staff_Reassignment
            modelBuilder.Entity<StaffAssignment>()
                .HasMany(a => a.StaffReassignments)
                .WithOne(r => r.StaffAssignment)
                .HasForeignKey(r => r.AssignmentId)
                .OnDelete(DeleteBehavior.Restrict);

           
        }
    }
}
