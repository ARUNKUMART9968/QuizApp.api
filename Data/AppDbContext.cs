using Microsoft.EntityFrameworkCore;
using QuizApp.Api.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace QuizApp.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Result> Results { get; set; } // FIXED: was 'r'

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasMaxLength(20);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            // Quiz Configuration
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.HasKey(e => e.QuizId);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.HasOne(q => q.Creator)
                      .WithMany(u => u.CreatedQuizzes)
                      .HasForeignKey(q => q.CreatedBy)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Question Configuration
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.QuestionId);
                entity.Property(e => e.Text).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Type).IsRequired().HasMaxLength(20);
                entity.Property(e => e.CorrectAnswer).IsRequired().HasMaxLength(500);
                entity.HasOne(q => q.Quiz)
                      .WithMany(qz => qz.Questions)
                      .HasForeignKey(q => q.QuizId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Options).HasColumnType("nvarchar(max)");
            });

            // Answer Configuration
            modelBuilder.Entity<Answer>(entity =>
            {
                entity.HasKey(e => e.AnswerId);
                entity.Property(e => e.SelectedAnswer).HasMaxLength(500);
                entity.HasOne(a => a.User)
                      .WithMany(u => u.Answers)
                      .HasForeignKey(a => a.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Quiz)
                      .WithMany(q => q.Answers)
                      .HasForeignKey(a => a.QuizId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(a => a.Question)
                      .WithMany(q => q.Answers)
                      .HasForeignKey(a => a.QuestionId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Result Configuration - FIXED
            modelBuilder.Entity<Result>(entity =>
            {
                entity.HasKey(e => e.ResultId);
                entity.HasOne(r => r.User)
                      .WithMany(u => u.Results)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(r => r.Quiz)
                      .WithMany(q => q.Results)
                      .HasForeignKey(r => r.QuizId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(e => e.Score).HasColumnType("decimal(5,2)");
            });
        }
    }
}