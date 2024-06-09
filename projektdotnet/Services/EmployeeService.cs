using Microsoft.CodeAnalysis.CSharp.Syntax;
using projektdotnet.Models;
using projektdotnet.Repositories;
using System.Security.Claims;
using System.Web.Helpers;
using System.Web.Mvc;

namespace projektdotnet.Services
{
    public class EmployeeService
    {
        private readonly TicketRepository _ticketRepository;
        private readonly EmployeeRepository _employeeRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RoleRepository _roleRepository;
        IConfiguration _configuration;

        public EmployeeService (IConfiguration configuration,TicketRepository ticketRepository, EmployeeRepository employeeRepository, IHttpContextAccessor httpContextAccessor,RoleRepository roleRepository)
        {
            _ticketRepository = ticketRepository;
            _employeeRepository = employeeRepository;
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }
        public async Task<Employee> GetEmployeeByUsername(string username)
        {
            return await _employeeRepository.GetEmployeeByUsername(username);    
        }
        public async Task<List<Employee>> GetAllEmployees()
        {
            return  await _employeeRepository.GetAllEmployees();
        }
        public async Task<Employee> GetEmployeeById(int? id)
        {
            if(id == null)
            {
                return null;
            }
            return await _employeeRepository.GetEmployeeById(id);
        }
        public async Task<Employee> GetEmployeeFromHttp()
        {
            var username = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if(username!=null)
            {
                return await _employeeRepository.GetEmployeeByUsername(username);
            }
            return null;
        }
        public async Task<Employee> SaltHashRole(Employee employee)
        {
            
            employee.Password = Crypto.HashPassword(string.Concat(employee.Password, _configuration.GetSection("salt").Value));
            employee.Roles.Add(await _roleRepository.GetRoleNormal());
            return employee;
        }
        
        public async Task AddEmployee(Employee employee)
        {
            await _employeeRepository.AddEmployee(employee);
        }
        public async Task UpdateEmployee(Employee employee)
        {
            await _employeeRepository.UpdateEmployee(employee);
        }
        public async Task RemoveEmployee(Employee employee)
        {
            await _employeeRepository.RemoveEmployee(employee);
        }
        public async Task<bool> EmployeeExists(int id)
        {
            return await _employeeRepository.EmployeeExist(id);
        }
        public async Task<Employee> GetEmployeeWithLeastTickets()
        {
            return await _employeeRepository.GetEmployeeWithLeastTickets();
        }
        public async Task<List<SelectListItem>> GetEmployeesForSelectList(string roleName)
        {
            var employees = await _employeeRepository.GetEmployeesWithRole(roleName);
            return employees.Select(e => new SelectListItem
            {
                Value = e.EmployeeId.ToString(),
                Text = $"{e.Name} {e.Surname} ID:{e.EmployeeId}"
            }).ToList();

        }
        public async Task<bool> EmployeeWithLoginExist(string Login)
        {
            return await _employeeRepository.EmployeeWithLoginExist(Login);
        }
    }

}
