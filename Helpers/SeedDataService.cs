using QuizApp.Api.Data;
using QuizApp.Api.Models;

namespace QuizApp.Api.Helpers
{
    public static class SeedDataService
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Seed admin user if not exists
            if (!context.Users.Any(u => u.Role == "Admin"))
            {
                var adminUser = new User
                {
                    Name = "System Admin",
                    Email = "admin@quizapp.com",
                    PasswordHash = PasswordHasher.HashPassword("Admin123!"),
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }

            // Seed sample student if not exists
            if (!context.Users.Any(u => u.Role == "Student"))
            {
                var studentUser = new User
                {
                    Name = "John Doe",
                    Email = "student@quizapp.com",
                    PasswordHash = PasswordHasher.HashPassword("Student123!"),
                    Role = "Student",
                    CreatedAt = DateTime.UtcNow
                };

                context.Users.Add(studentUser);
                await context.SaveChangesAsync();
            }
        }
    }
}