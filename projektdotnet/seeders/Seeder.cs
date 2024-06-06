using Org.BouncyCastle.Security;
using projektdotnet.Data;
using projektdotnet.Models;
using System.Web.Helpers;

namespace projektdotnet.wwwroot.seeders
{
    public class Seeder
    {
        private readonly NewDbContext _context;
        private readonly IConfiguration _configuration;
        public Seeder(NewDbContext context,IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task seedRoles()
        {
            if (!_context.Roles.Any()) {
                var ADMIN = new Role()
                {
                    Name = "ADMIN"
                };
                var HR = new Role() 
                { 
                    Name = "HR"
                };
                var NORMAL = new Role()
                {
                    Name = "NORMAL"
                };
                _context.Roles.Add(ADMIN);
                _context.Roles.Add(HR);
                _context.Roles.Add(NORMAL);
                await _context.SaveChangesAsync();
            }
        }
        public async Task seedHR()
        {
            if(!_context.Employees.Any())
            {
                var hrRole = _context.Roles.FirstOrDefault(r => r.Name == "HR");
                var normalRole = _context.Roles.FirstOrDefault(r => r.Name == "NORMAL");
                var employee = new Employee()
                {
                    Login = "hrhrhr",
                    Password = Crypto.HashPassword(_configuration.GetSection("hrpassword").Value),
                    Name = "Hrowiec",
                    Surname = "Hrowski",
                    Roles =  new List<Role> { hrRole,normalRole}
                };
                _context.Add(employee);
                await _context.SaveChangesAsync();
            }
        }
    }
}
