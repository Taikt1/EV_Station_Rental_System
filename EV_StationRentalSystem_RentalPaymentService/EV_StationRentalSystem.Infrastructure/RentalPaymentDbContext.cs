using EV_StationRentalSystem.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure
{
    public class RentalPaymentDbContext : DbContext
    {
        public RentalPaymentDbContext(DbContextOptions<RentalPaymentDbContext> options) : base(options) { }

        public DbSet<RentalOrder> RentalOrders { get; set; }
        public DbSet<RentalOrderDetail> RentalOrderDetails { get; set; }
        public DbSet<RentalContract> RentalContracts { get; set; }
        public DbSet<Checkin> Checkins { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<PhotoProof> PhotoProofs { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PenaltyRecord> PenaltyRecords { get; set; }
        public DbSet<FeedbackRating> FeedbackRatings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Rental_Order 1:1 Rental_Contract
            modelBuilder.Entity<RentalOrder>()
                .HasOne(o => o.RentalContract)
                .WithOne(c => c.RentalOrder)
                .HasForeignKey<RentalContract>(c => c.RentalId);

            // Rental_Order 1:N Payment
            modelBuilder.Entity<RentalOrder>()
                .HasMany(o => o.Payments)
                .WithOne(p => p.RentalOrder)
                .HasForeignKey(p => p.RentalId);

            // Rental_Order 1:N Penalty_Record
            modelBuilder.Entity<RentalOrder>()
                .HasMany(o => o.PenaltyRecords)
                .WithOne(pr => pr.RentalOrder)
                .HasForeignKey(pr => pr.RentalId);

            // Rental_Order 1:N Feedback_Rating
            modelBuilder.Entity<RentalOrder>()
                .HasMany(o => o.FeedbackRatings)
                .WithOne(f => f.RentalOrder)
                .HasForeignKey(f => f.RentalId);

            // Checkin 1:N PhotoProof
            modelBuilder.Entity<Checkin>()
                .HasMany(c => c.PhotoProofs)
                .WithOne(p => p.Checkin)
                .HasForeignKey(p => p.CheckinId)
                .OnDelete(DeleteBehavior.Restrict);


            // RentalOrder 1:N RentalOrderDetail
            modelBuilder.Entity<RentalOrder>()
                .HasMany(o => o.RentalOrderDetails)
                .WithOne(d => d.RentalOrder)
                .HasForeignKey(d => d.RentalOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // RentalOrderDetail 1:N Checkin
            modelBuilder.Entity<RentalOrderDetail>()
                .HasMany(d => d.Checkins)
                .WithOne(c => c.RentalOrderDetail)
                .HasForeignKey(c => c.RentalOrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            // RentalOrderDetail 1:N Checkout
            modelBuilder.Entity<RentalOrderDetail>()
                .HasMany(d => d.Checkouts)
                .WithOne(c => c.RentalOrderDetail)
                .HasForeignKey(c => c.RentalOrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);

            // Checkout 1:N PhotoProof
            modelBuilder.Entity<Checkout>()
                .HasMany(c => c.PhotoProofs)
                .WithOne(p => p.Checkout)
                .HasForeignKey(p => p.CheckoutId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
