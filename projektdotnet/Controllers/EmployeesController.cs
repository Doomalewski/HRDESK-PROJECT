using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NuGet.Packaging;
using projektdotnet.Data;
using projektdotnet.Models;
using projektdotnet.Services;

namespace projektdotnet.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly TicketService _ticketService;
        private readonly EmployeeService _employeeService;
        private readonly EmailService _emailService;
        public EmployeesController(TicketService ticketService,EmployeeService employeeService,EmailService emailService)
        {
            _ticketService = ticketService;
            _employeeService = employeeService;
            _emailService = emailService;
        }

        // GET: Employees
        [Route("Employees")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Index()
        {
            return View(await _employeeService.GetAllEmployees());
        }
        // GET: Employees/Details/5
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Details()
        {
            var user = await _employeeService.GetEmployeeFromHttp();
            if (user!=null)
            {
                return View(user);
            }
            return NotFound();
        }
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> EmployeeDetails(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            var profilePicturePath = $"/Files/{employee.Login}profilepicture.jpg";
            var defaultProfilePicturePath = "/Files/defaultprofilepicture.jpg";

            // Map the URL path to the physical file location
            var profilePicturePhysicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", $"{employee.Login}profilepicture.jpg");

            // Check if the physical file exists
            var profilePictureToUse = System.IO.File.Exists(profilePicturePhysicalPath) ? profilePicturePath : defaultProfilePicturePath;

            ViewBag.PathToProfilePicture = profilePictureToUse;
            return View(employee);
        }
        // GET: Employees/Create
        [Authorize(Roles = "HR")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeesCreate
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Login,Password,Name,Surname")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                if(await _employeeService.EmployeeWithLoginExist(employee.Login))
                {
                    return View("EmployeeExist");
                }
                await _emailService.SendEmail(employee.Name,employee.Login,employee.Password);
                employee = await _employeeService.SaltHashRole(employee);
                await _employeeService.AddEmployee(employee);
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }
        // GET: EMployees/Edit/5
        [Authorize(Roles = "HR")]

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,Login,Password,Name,Surname")] Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployee(employee);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _employeeService.EmployeeExists(employee.EmployeeId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [Authorize(Roles = "HR")]
        // GET: Employees/Delete/5
        [Route("Layoff/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }
        [Authorize(Roles = "HR")]
        // POST: Employees/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _employeeService.GetEmployeeById(id);
            //setting SenderId in SentTickets for null
            await _ticketService.NullifySenderTickets(employee);

            if (employee != null)
            {
                //if HR employee deleted then move his Received tickets
                if (employee.Roles.Any(r => r.Name == "HR"))
                {
                    await _ticketService.MoveReceivedTickets(employee);
                    await _employeeService.RemoveEmployee(employee);
                    return RedirectToAction(nameof(Index));
                }
                await _employeeService.RemoveEmployee(employee);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
