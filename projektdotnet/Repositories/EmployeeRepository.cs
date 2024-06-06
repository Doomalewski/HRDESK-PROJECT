using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using System.Security.Policy;

namespace projektdotnet.Repositories
{
    public class EmployeeRepository
    {
        private readonly NewDbContext _context;
        public EmployeeRepository(NewDbContext context)
        {
            _context = context;
        }
        public async Task<List<Employee>> GetAllEmployees()
        {
            return await _context.Employees.Include(m => m.Meetings)
                .Include(s => s.SentTickets)
                .Include(r => r.ReceivedTickets)
                .Include(r => r.Roles).ToListAsync();
        }
        public async Task<Employee> GetEmployeeById(int? id)
        {
            if (id == null)
            {
                return null;
            }
            return await _context.Employees
                    .Include(m => m.Meetings)
                    .Include(s => s.SentTickets)
                    .Include(r => r.ReceivedTickets)
                    .Include(r => r.Roles)
                    .FirstOrDefaultAsync(m => m.EmployeeId == id);
        }
        public async Task<Employee> GetEmployeeWithLeastTicketsExId(int id)
        {
            return await _context.Employees
                            .Include(m => m.Meetings)
                            .Include(s => s.SentTickets)
                            .Include(r => r.ReceivedTickets)
                            .Include(r => r.Roles)
                            .Where(e => e.Roles.Any(r => r.Name == "HR") && e.EmployeeId != id)
                            .OrderBy(e => e.ReceivedTickets.Count)
                            .FirstOrDefaultAsync();
        }
        public async Task<Employee> GetEmployeeWithLeastTickets()
        {
            return await _context.Employees
                            .Include(m => m.Meetings)
                            .Include(s => s.SentTickets)
                            .Include(r => r.ReceivedTickets)
                            .Include(r => r.Roles)
                            .Where(e => e.Roles.Any(r => r.Name == "HR"))
                            .OrderBy(e => e.ReceivedTickets.Count)
                            .FirstOrDefaultAsync();
        }
        public async Task<bool> EmployeeExist(int id)
        {
            return await _context.Employees
                .AnyAsync(e => e.EmployeeId == id);
        }
        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            return await _context.Employees
                .Include(m => m.Meetings)
                .Include(s => s.SentTickets)
                .Include(r => r.ReceivedTickets)
                .Include(r => r.Roles).FirstOrDefaultAsync(e=>e.Login==username);
        }

        public async Task AddEmployee(Employee employee)
        {
            _context.Add(employee);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateEmployee(Employee employee)
        {
            _context.Update(employee);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveEmployee(Employee employee)
        {
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Employee>> GetEmployeesWithRole(string roleName)
        {
            return await _context.Employees.Where(r=>r.Roles.Any(r=>r.Name==roleName)).ToListAsync();
        }
        public async Task<bool> EmployeeWithLoginExist(string Login)
        {
            if (await _context.Employees.AnyAsync(e => e.Login == Login))
            {
                return true;
            }
            return false;
        }
    }
}
