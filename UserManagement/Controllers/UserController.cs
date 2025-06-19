using Microsoft.AspNetCore.Mvc;
using BOL.DTOs;
using BLL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Infrastructure;



namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly AuthService _authService;
        public IActionResult Index()
        {
            return View();
        }





        public UserController(AuthService authService)
        {
            this._authService = authService;
        }






        [HttpPost("/register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO  dto)
        {

            Console.WriteLine("DTO +> "+dto);
            
            var result = await this._authService.RegisterAsync(dto);
            Console.WriteLine(result);
            if ( result == "registered")
            {
              return Ok();
            }
            return BadRequest(result);
        }


        [HttpGet("register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login ([FromBody] LoginDTO dto)
        {
            Console.WriteLine(dto.Password+"  "+dto.Email );
            var result = await  this._authService.LoginAsync(dto);
            return result !=null?  Ok(result):BadRequest("problem") ;
        }








        [HttpGet("myprofile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) { 
            
            return Unauthorized();
            
            }

            int userId = int.Parse(userIdClaim.Value);

            var profile = await _authService.GetProfileDTOAsync(userId);

            return profile == null ? NotFound("User not found ") : Ok(profile);
        }


        [HttpGet("ListUser")]

        public   IActionResult UserList()
        {
            return View();
        }
        [HttpGet("AllUsers")]
        public async Task<IActionResult> AllUsers()
        {
           var users=  await _authService.ListUsers();
            return Ok(users);
        }


        //[Authorize]
        [HttpGet("detail/{id}")]
        public async Task<IActionResult> Profile(int id)
        {
            var user = await _authService.GetProfileById(id);
            Console.WriteLine(user  + " <= user " );
            return user != null? Ok(user) :BadRequest("user not found")   ;
        }


        //[Authorize]
        [HttpGet("profile/{id}")]

        public IActionResult UserProfileDetail(int id) {

            return View("Profile",id);
        }

        [HttpGet("update/{id}")]
        public IActionResult UpdateProfile(int id)
        {
            return View("UpdateUserProfile", id);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUserProfile(int id, [FromBody] UpdateProfileDTO dto)
        {
           string? result = await _authService.UpdateUserProfile( id, dto); 
            return Ok(result);
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            string? result =  await this._authService.DeleteUser(id);

            return  result !=null? Ok(result) :BadRequest("User not Found");
        }


        [HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // Show Reset Password form (with token and email from query string)
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }



        [HttpGet("create-password")]
        public IActionResult CreatePassword(string email, string token)
        {
            ViewBag.Email = email;
            ViewBag.Token = token;
            return View();
        }

        //[HttpPost("create-password")]
        //public async Task<IActionResult> CreatePassword([FromBody] ForgotPasswordDTO dto)
        //{
        //    var result = await this._authService.SendResetEmailAsync(dto.Email);
        //    return Ok(result);
        //}


        [HttpPost("forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO dto)
        {
            var result = await this._authService.SendResetEmailAsync(dto.Email);
            return Ok(result);
        }

        [HttpPost("reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO dto)
        {
            var result = await this._authService.ResetPasswordAsync(dto);
            return Ok(result);
        }
    }
}
