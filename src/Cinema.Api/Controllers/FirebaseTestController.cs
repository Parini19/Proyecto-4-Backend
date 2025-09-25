using System.Linq;
using System.Threading.Tasks;
using Cinema.Api.Services;
using Cinema.Domain.Entities;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Cinema.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FirebaseTestController : ControllerBase
    {
        private readonly string _firebaseConfigPath;
        private readonly UserService _userService;

        public FirebaseTestController(IConfiguration configuration)
        {
            _firebaseConfigPath = configuration["Firebase:ConfigPath"];
            _userService = new UserService(configuration["Firebase:ConfigPath"]);
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
        public async Task<IActionResult> AddUser()
        {
            try
            {
                var user = new User
                {
                    Email = "testuser@example.com",
                    DisplayName = "Test User",
                    EmailVerified = false,
                    Disabled = false
                };
                var createdUser = await _userService.CreateUserAsync(user, "TestPassword123!");
                return Ok(new { success = true, uid = createdUser.Uid });
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
                EnsureFirebaseInitialized();
                var userRecord = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                return Ok(new
                {
                    success = true,
                    user = new
                    {
                        userRecord.Uid,
                        userRecord.Email,
                        userRecord.DisplayName,
                        userRecord.EmailVerified,
                        userRecord.Disabled,
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
                EnsureFirebaseInitialized();
                await FirebaseAuth.DefaultInstance.DeleteUserAsync(uid);
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
                var updatedUser = await _userService.UpdateUserAsync(user);
                return Ok(new { success = true, user = updatedUser });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to edit user.", error = ex.Message });
            }
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.ListUsersAsync();
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
                    }),
                });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Failed to get users.", error = ex.Message });
            }
        }
    }
}