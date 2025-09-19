using BCrypt.Net;
using System.Diagnostics.Eventing.Reader;

namespace QuizApp.Api.Helpers
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            return BCrypt.EnhancedHashPassword(password, HashType.SHA384);
        }

        public static bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.EnhancedVerify(password, hash, HashType.SHA384);
            }
            catch
            {
                return false;
            }
        }
    }
}
