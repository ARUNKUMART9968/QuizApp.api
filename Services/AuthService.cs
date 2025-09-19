// Services/AuthService.cs
using QuizApp.Api.DTOs;
using QuizApp.Api.Models;
using QuizApp.Api.Repositories;
using QuizApp.Api.Helpers;
using QuizApp.Api.Repositories.Interfaces;

namespace QuizApp.Api.Services
{
    public class AuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user exists
            if (await _userRepository.ExistsByEmailAsync(registerDto.Email))
            {
                throw new ArgumentException("User with this email already exists");
            }

            // Validate role
            if (registerDto.Role != "Admin" && registerDto.Role != "Student")
            {
                throw new ArgumentException("Role must be either 'Admin' or 'Student'");
            }

            // Create user
            var user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email.ToLower(),
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.CreateAsync(user);
            var token = _jwtHelper.GenerateToken(createdUser);

            return new AuthResponseDto
            {
                Token = token,
                User = MapToUserDto(createdUser)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email.ToLower());
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Verify password
            if (!PasswordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token = _jwtHelper.GenerateToken(user);

            return new AuthResponseDto
            {
                Token = token,
                User = MapToUserDto(user)
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return MapToUserDto(user);
        }

        private static UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };
        }
    }
}