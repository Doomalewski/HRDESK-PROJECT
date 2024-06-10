using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using projektdotnet.Services;
using System.Diagnostics;
using System.Security.Claims;
using System.Web.Helpers;

namespace projektdotnet.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MeetingService _meetingService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmployeeService _employeeService;
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration,ILogger<HomeController> logger,MeetingService meetingService, IHttpContextAccessor httpContextAccessor, EmployeeService employeeService)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _employeeService = employeeService;
            _configuration = configuration;
            _meetingService = meetingService;
        }
        [Authorize(Roles = "NORMAL")]
        [Route("FrequentlyAskedQuestions")]
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string ReturnUrl)
        {
            var employee = await _employeeService.GetEmployeeByUsername(username);
            if (employee == null)
            {
                return View();
            }
            var saltedInputPassword = string.Concat(password, _configuration.GetSection("salt").Value);
            if (Crypto.VerifyHashedPassword(employee.Password, saltedInputPassword))
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, employee.Login));
                identity.AddClaim(new Claim(ClaimTypes.Name, employee.Name));
                identity.AddClaim(new Claim(ClaimTypes.Surname, employee.Surname));
                foreach (var role in employee.Roles)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, role.Name));
                }
                var claims = identity;
                var principal = new ClaimsPrincipal(claims);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                return Redirect(ReturnUrl == null ? "/Home/Index" : ReturnUrl);
            }
            else
            {
                return View();
            }
        }
        [HttpGet]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        public IActionResult DeniedAccess()
        {
            return View();
        }
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Index()
        {
            
            var User = await _employeeService.GetEmployeeFromHttp();
            var todaysMeetings = await _meetingService.GetTodaysMeetingsForEmployee(User.EmployeeId);
            if (User.Roles.Any(r => r.Name == "HR"))
            {
                return View("IndexHR",todaysMeetings);
            }
            else 
            { 
                return View("Index",todaysMeetings);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
