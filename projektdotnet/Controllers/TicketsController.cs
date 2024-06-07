using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using projektdotnet.Data;
using projektdotnet.Models;
using projektdotnet.Services;

namespace projektdotnet.Controllers
{
    public class TicketsController : Controller
    {

        private readonly NewDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TicketService _ticketService;
        private readonly EmployeeService _employeeService;
        public TicketsController(NewDbContext context, IHttpContextAccessor httpContextAccessor, TicketService ticketService, EmployeeService employeeService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _ticketService = ticketService;
            _employeeService = employeeService;
        }

        // GET: Tickets
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Index()
        {
            var User = await _employeeService.GetEmployeeFromHttp();
            var ticketsHR = await _ticketService.GetTicketsWithReceiverById(User.EmployeeId);
            var resolvedTicketsCount = ticketsHR.Count(t => t.Status == TicketStatus.Resolved);
            var AllResolvedTickets = await _ticketService.GetAllResolvedTickets();
            ViewBag.resolvedTicketsCount = resolvedTicketsCount;
            ViewBag.partOfAllTicketsResolved = Math.Round(((double)resolvedTicketsCount / AllResolvedTickets.Count) * 100, 2);
            var ticketsNormal = await _ticketService.GetTicketsWithSenderById(User.EmployeeId);
            ticketsHR.RemoveAll(t => t.Status == TicketStatus.Resolved);
            if (User.Roles.Any(r => r.Name == "HR"))
            {
                return View("TicketsHR", ticketsHR);
            }
            else
            {
                return View("Tickets", ticketsNormal);
            }
        }
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> ReadyTicketsPanel()
        {
            ViewData["Receivers"] = new SelectList(await _employeeService.GetEmployeesForSelectList("HR"), "Value", "Text");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> ReadyTicketsPanel(int id)
        {
            Ticket newTicket = _ticketService.MakeReadyTicket(id);
            var employee = await _employeeService.GetEmployeeFromHttp();
            newTicket.SenderId = employee.EmployeeId;
            var receiver = await _employeeService.GetEmployeeWithLeastTickets();
            newTicket.ReceiverId = receiver.EmployeeId;
            if (ModelState.IsValid)
            {
                await _ticketService.AddTicket(newTicket);
            }
            return RedirectToAction("Index");
        }
        [Authorize(Roles = "HR")]
        public async Task<ViewResult> ResolvedTickets()
        {
            var user = await _employeeService.GetEmployeeFromHttp();
            var tickets = await _ticketService.GetTicketsWithReceiverById(user.EmployeeId);
            var ticketsResolved = tickets.Where(t => t.Status == TicketStatus.Resolved).ToList();

            return View(ticketsResolved);
        }
        // GET: Tickets/Details/5
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _employeeService.GetEmployeeFromHttp();
            ViewBag.Role = user.Roles.Any(r => r.Name == "HR");
            ViewBag.userId = user.EmployeeId;
            if (!user.SentTickets.Any(e => e.TicketId == id) && !user.ReceivedTickets.Any(e => e.TicketId == id))
            {
                return RedirectToAction("DeniedAccess", "Home");
            }

            var ticket = await _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        // GET: Tickets/Create
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> CreateAsync()
        {
            ViewData["Receivers"] = new SelectList(await _employeeService.GetEmployeesForSelectList("HR"), "Value", "Text");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Create([Bind("ReceiverId,Category,Status,Priority,CreationDate,Description")] Ticket ticket, bool ReceiverChosen)
        {

            if (ModelState.IsValid)
            {

                var user = await _employeeService.GetEmployeeFromHttp();
                if (user != null)
                {
                    ticket.SenderId = user.EmployeeId;
                }
                if (!ReceiverChosen)
                {
                    var HrWithLeastTickets = await _employeeService.GetEmployeeWithLeastTickets();
                }

                await _ticketService.AddTicket(ticket);
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketById(id);
            if (ticket == null)
            {
                return NotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "HR")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SenderId, ReceiverId,Category,Status,Priority,CreationDate,Description")] Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketService.UpdateTicket(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ticketService.TicketExists(ticket.TicketId))
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
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [Authorize(Roles = "HR")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _ticketService.GetTicketById(id);

            if (ticket != null)
            {
                await _ticketService.RemoveTicket(ticket);
            }

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Manage(int id)
        {
            var ticket = await _ticketService.GetTicketById(id);

            if (ticket == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeFromHttp();
            var employees = await _employeeService.GetEmployeesForSelectList("HR");

            employees.RemoveAll(e => e.Value == employee.EmployeeId.ToString());

            ViewData["Employees"] = new SelectList(employees, "Value", "Text");

            return View(ticket);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Manage(int id, Ticket model, bool changeReceiver)
        {
            if (id != model.TicketId)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(r => r.Receiver)
                .Include(s => s.Sender)
                .FirstOrDefaultAsync(t => t.TicketId == id);

            if (ticket == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (changeReceiver)
                    {
                        ticket.ReceiverId = model.ReceiverId;
                    }

                    ticket.Status = model.Status;
                    ticket.Description = model.Description;  // Przypisanie opisu z modelu do biletu
                    ticket.Priority = model.Priority;
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ticketService.TicketExists(model.TicketId))
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

            var user = _httpContextAccessor.HttpContext.User;
            var currentUserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var employees = await _context.Employees
                .Where(e => e.Roles.Any(r => r.Name == "HR") && e.EmployeeId.ToString() != currentUserId)
                .Select(e => new { e.EmployeeId, FullName = e.Name + " " + e.Surname })
                .ToListAsync();

            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName");

            return View(model);
        }



    }
}