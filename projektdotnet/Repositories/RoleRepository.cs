using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;

namespace projektdotnet.Repositories
{
    public class RoleRepository : Controller
    {
        private readonly NewDbContext _context;

        public RoleRepository(NewDbContext context)
        {
            _context = context;
        }
        public async Task<Role> GetRoleNormal()
        {
            return await _context.Roles.FirstOrDefaultAsync(r => r.Name == "NORMAL");
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
