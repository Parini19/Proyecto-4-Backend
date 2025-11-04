using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.FeatureManagement.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseTestController : ControllerBase
    {
        private readonly string _firebaseConfigPath;
        private readonly FirestoreUserService _firestoreUserService;

        public FirebaseTestController(IConfiguration configuration, FirestoreUserService firestoreUserService)
        {
            _firebaseConfigPath = configuration["Firebase:ConfigPath"];
            _firestoreUserService = firestoreUserService;
        }

        private void EnsureFirebaseInitialized()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_firebaseConfigPath)
                });
            }
        }

        [HttpGet("test-connection")]
        [FeatureGate("DatabaseConnection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                EnsureFirebaseInitialized();
                return Ok(new { success = true, message = "Firebase connection is valid." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Firebase connection failed.", error = ex.Message });
            }
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            try
            {
                await _firestoreUserService.AddUserAsync(user);
                return Ok(new { success = true, uid = user.Uid });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to add user.", error = ex.Message });
            }
        }

        [HttpGet("get-user/{uid}")]
        public async Task<IActionResult> GetUser(string uid)
        {
            try
            {
                var user = await _firestoreUserService.GetUserAsync(uid);
                if (user == null)
                    return NotFound(new { success = false, message = "User not found." });

                return Ok(new
                {
                    success = true,
                    user = new
                    {
                        user.Uid,
                        user.Email,
                        user.DisplayName,
                        user.EmailVerified,
                        user.Disabled,
                        user.Role
                    },
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get user.", error = ex.Message });
            }
        }

        [HttpDelete("delete-user/{uid}")]
        public async Task<IActionResult> DeleteUser(string uid)
        {
            try
            {
                await _firestoreUserService.DeleteUserAsync(uid);
                return Ok(new { success = true, message = $"User {uid} deleted." });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to delete user.", error = ex.Message });
            }
        }

        [HttpPut("edit-user/{uid}")]
        public async Task<IActionResult> EditUser(string uid, [FromBody] User user)
        {
            try
            {
                user.Uid = uid;
                await _firestoreUserService.UpdateUserAsync(user);
                return Ok(new { success = true, user });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to edit user.", error = ex.Message });
            }
        }

        [HttpGet("get-all-users")]
        [FeatureGate("DatabaseReadAll")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _firestoreUserService.GetAllUsersAsync();
                return Ok(new
                {
                    success = true,
                    users = users.Select(u => new
                    {
                        u.Uid,
                        u.Email,
                        u.DisplayName,
                        u.EmailVerified,
                        u.Disabled,
                        u.Role
                    }),
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get users.", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                // Verify credentials using Firestore
                var user = await _firestoreUserService.GetUserByEmailAsync(loginDto.Email);
                if (user == null)
                    return Unauthorized(new { success = false, message = "User not found." });

                var isValid = await _firestoreUserService.VerifyUserPasswordAsync(loginDto.Email, loginDto.Password);
                if (!isValid)
                    return Unauthorized(new { success = false, message = "Invalid credentials." });

                var jwtToken = GenerateJwtToken(user, HttpContext.RequestServices.GetService<IConfiguration>());

                return Ok(new
                {
                    success = true,
                    uid = user.Uid,
                    email = user.Email,
                    displayName = user.DisplayName,
                    role = user.Role,
                    jwtToken
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Login failed.", error = ex.Message });
            }
        }

        // Helper method remains unchanged
        private string GenerateJwtToken(User user, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Uid),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role ?? "user"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpiresMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [Authorize(Roles = "admin")]
        [HttpGet("motd")]
        public IActionResult GetMessageOfTheDay()
        {
            var message = "Welcome to Cinema! Today's special: 2x1 tickets for admins.";
            return Ok(new { success = true, message });
        }
    }
}