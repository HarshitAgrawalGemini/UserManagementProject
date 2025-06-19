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
        private readonly PasswordService _passwordService;
        private readonly UserService _userService;
        public IActionResult Index()
        {
            return View();
        }





        public UserController(AuthService authService, PasswordService passwordService, UserService userService)
        {
            this._authService = authService;
            this._passwordService = passwordService;
            this._userService = userService;
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







        [HttpGet("AllUsers")]
        [Authorize]
        public async Task<IActionResult> AllUsers()
        {
            var users = await _userService.ListUsers();
            return Ok(users);
        }



        [HttpGet("ListUser")]
        //[Authorize]

        public   IActionResult UserList()
        {
            return View();
        }


        //[Authorize]


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



    }
}
