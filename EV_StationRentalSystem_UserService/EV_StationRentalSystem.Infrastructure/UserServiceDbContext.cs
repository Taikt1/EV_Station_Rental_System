using EV_StationRentalSystem.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure
{
    public class UserServiceDbContext : IdentityDbContext<ApplicationUser>
    {
        public UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : base(options) { }

        public DbSet<ApplicationUser> Accounts { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<SystemLog> SystemLogs { get; set; }
        public DbSet<PersonalAnalytics> PersonalAnalytics { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Account - UserProfile (1:1)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(a => a.UserProfile)
                .WithOne(p => p.Account)
                .HasForeignKey<UserProfile>(p => p.UserId);

            // UserProfile - Notification (1:N)
            modelBuilder.Entity<UserProfile>()
                .HasMany(p => p.Notifications)
                .WithOne(n => n.UserProfile)
                .HasForeignKey(n => n.UserId);

            // UserProfile - PersonalAnalytics (1:1)
            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.PersonalAnalytics)
                .WithOne(a => a.UserProfile)
                .HasForeignKey<PersonalAnalytics>(a => a.UserId);

            // Account - SystemLog (1:N)
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(a => a.SystemLogs)
                .WithOne(l => l.Account)
                .HasForeignKey(l => l.UserId);

            // Seeding Data
            modelBuilder.Entity<IdentityRole>().HasData(SeedingRoles());
        }

        private ICollection<IdentityRole> SeedingRoles()
        {
            return new List<IdentityRole>()
        {
            new IdentityRole() {
                Id = "3631e38b-60dd-4d1a-af7f-a26f21c2ef82",
                Name = "manager",
                NormalizedName = "MANAGER",
                ConcurrencyStamp = "3631e38b-60dd-4d1a-af7f-a26f21c2ef82"
            },
            new IdentityRole() {
                Id = "51ef7e08-ff07-459b-8c55-c7ebac505103",
                Name = "staff",
                NormalizedName = "STAFF",
                ConcurrencyStamp = "51ef7e08-ff07-459b-8c55-c7ebac505103"
            },
            new IdentityRole() {
                Id = "37a7c5df-4898-4fd4-8e5f-d2abd4b57520",
                Name = "customer",
                NormalizedName = "CUSTOMER",
                ConcurrencyStamp = "37a7c5df-4898-4fd4-8e5f-d2abd4b57520"
            }
        };
        }
    }


}
