using Microsoft.AspNetCore.Mvc;
using BOL.DTOs;
using BLL.Services;



namespace UserManagement.Controllers
{
    [ApiController]
    [Route("api/controller")]
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
        [HttpPost("register")]
        public async Task<IActionResult> Register( RegisterDTO  dto)
        {
            var result = await this._authService.RegisterAsync(dto);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await  this._authService.LoginAsync(dto);
            return Ok(result);
        }
    }
}
