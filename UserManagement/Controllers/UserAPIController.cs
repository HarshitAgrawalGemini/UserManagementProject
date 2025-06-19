using BLL.Services;
using BOL.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace UserManagement.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserAPIController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;
        private readonly UserService _userService;

         public  UserAPIController(AuthService authService, PasswordService passwordService, UserService userService)
        {
            this._authService = authService;
            this._passwordService = passwordService;
            this._userService = userService;
        }


        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {

            Console.WriteLine("DTO +> " + dto);

            var result = await this._authService.RegisterAsync(dto);
            Console.WriteLine(result);
            if (result == "registered")
            {
                return Ok();
            }
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            Console.WriteLine(dto.Password + "  " + dto.Email);
            var result = await this._authService.LoginAsync(dto);
            return result != null ? Ok(result) : BadRequest("problem");
        }


        [HttpGet("myprofile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {

                return Unauthorized();

            }

            int userId = int.Parse(userIdClaim.Value);

            var profile = await _userService.GetProfileDTOAsync(userId);

            return profile == null ? NotFound("User not found ") : Ok(profile);
        }
        [HttpGet("AllUsers")]
        public async Task<IActionResult> AllUsers()
        {
            var users = await _userService.ListUsers();
            return Ok(users);
        }
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _userService.GetProfileById(id);
            Console.WriteLine(user + " <= user ");
            return user != null ? Ok(user) : BadRequest("user not found");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateProfileDTO dto)
        {
            string? result = await _userService.UpdateUserProfile(id, dto);
            return Ok(result);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string? result = await this._userService.DeleteUser(id);

            return result != null ? Ok(result) : BadRequest("User not Found");
        }
        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var result = await this._passwordService.SendResetEmailAsync(dto.Email);
            return Ok(result);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var result = await this._passwordService.ResetPasswordAsync(dto);
            return Ok(result);
        }
    }
}
