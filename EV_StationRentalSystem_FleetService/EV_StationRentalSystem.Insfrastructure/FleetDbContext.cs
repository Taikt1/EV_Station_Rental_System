using EV_StationRentalSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure
{
    public class FleetDbContext : DbContext
    {
        public FleetDbContext(DbContextOptions<FleetDbContext> options) : base(options) { }

        public DbSet<BranchDestination> BranchDestinations { get; set; }
        public DbSet<TypeVehicle> TypeVehicles { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<VehicleSchedule> VehicleSchedules { get; set; }
        public DbSet<MaintenanceRecord> MaintenanceRecords { get; set; }
        public DbSet<ChargingRecord> ChargingRecords { get; set; }
        public DbSet<VehicleRelocation> VehicleRelocations { get; set; }
        public DbSet<VehicleAvailabilityByBranch> VehicleAvailabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Branch (1:N) Vehicle
            //modelBuilder.Entity<BranchDestination>()
            //    .HasMany(b => b.Vehicles)
            //    .WithOne(v => v.BranchDestination)
            //    .HasForeignKey(v => v.BranchId)
            //    .OnDelete(DeleteBehavior.Restrict);

            // Type_Vehicle (1:N) Vehicle
            modelBuilder.Entity<TypeVehicle>()
                .HasMany(t => t.Vehicles)
                .WithOne(v => v.TypeVehicle)
                .HasForeignKey(v => v.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle (1:N) Maintenance_Record
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.MaintenanceRecords)
                .WithOne(m => m.Vehicle)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle (1:N) Charging_Record
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.ChargingRecords)
                .WithOne(c => c.Vehicle)
                .HasForeignKey(c => c.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle (1:N) Vehicle_Schedule
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.VehicleSchedules)
                .WithOne(s => s.Vehicle)
                .HasForeignKey(s => s.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicle (1:N) Vehicle_Relocation
            modelBuilder.Entity<Vehicle>()
                .HasMany(v => v.VehicleRelocations)
                .WithOne(r => r.Vehicle)
                .HasForeignKey(r => r.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Branch (1:N) Vehicle_Availability
            modelBuilder.Entity<BranchDestination>()
                .HasMany(b => b.VehicleAvailabilities)
                .WithOne(a => a.BranchDestination)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
