using Microsoft.EntityFrameworkCore;
using Service_Apotheke.Models;
using ServiceApothekeAPI;

namespace ServiceApothekeAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Pharmacist> Pharmacists { get; set; }
        public DbSet<Pharmacy> Pharmacies { get; set; }
        public DbSet<JobPost> JobPosts { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Pharmacist
            modelBuilder.Entity<Pharmacist>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Pharmacist>()
                .Property(p => p.IsProfileCompleted)
                .HasDefaultValue(false);

            modelBuilder.Entity<Pharmacist>()
                .Property(p => p.IsEmailConfirmed)
                .HasDefaultValue(false);

            // Configure Pharmacy
            modelBuilder.Entity<Pharmacy>()
                .HasIndex(p => p.Email)
                .IsUnique();

            modelBuilder.Entity<Pharmacy>()
                .HasIndex(p => p.LicenseNumber)
                .IsUnique();

            modelBuilder.Entity<Pharmacy>()
                .Property(p => p.IsVerified)
                .HasDefaultValue(false);

            // Configure JobPost
            modelBuilder.Entity<JobPost>()
                .HasOne(jp => jp.Pharmacy)
                .WithMany(p => p.JobPosts)
                .HasForeignKey(jp => jp.PharmacyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure JobApplication
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobPost)
                .WithMany(jp => jp.JobApplications)
                .HasForeignKey(ja => ja.JobPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Pharmacist)
                .WithMany(p => p.JobApplications)
                .HasForeignKey(ja => ja.PharmacistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Shift
            modelBuilder.Entity<Shift>()
                .HasOne(s => s.JobPost)
                .WithMany()
                .HasForeignKey(s => s.JobPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Pharmacist)
                .WithMany(p => p.Shifts)
                .HasForeignKey(s => s.PharmacistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Notification
            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);
        }
    }
}