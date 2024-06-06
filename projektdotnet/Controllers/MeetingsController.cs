using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Model;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using projektdotnet.Data;
using projektdotnet.Models;
using projektdotnet.Services;

namespace projektdotnet.Controllers
{
    public class MeetingsController : Controller
    {
        private readonly NewDbContext _context;
        private readonly EmployeeService _employeeService;
        public MeetingsController(NewDbContext context,EmployeeService employeeService)
        {
            _employeeService = employeeService;
            _context = context;
        }

        // GET: Meetings
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Index()
        {
            var user = await _employeeService.GetEmployeeFromHttp();
            var newDbContext = await _context.Meetings.Include(m => m.room).Where(e=>e.Participants.Any(e=>e.EmployeeId==user.EmployeeId)).OrderBy(e=>e.StartingTime).ToListAsync();
            return View(newDbContext);
        }
        [HttpPost]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> LeaveMeeting(int id)
        {
            var meeting = await _context.Meetings.Where(m => m.MeetingId == id).FirstOrDefaultAsync();
            var user = await _employeeService.GetEmployeeFromHttp();

            user.Meetings.Remove(meeting);
            meeting.Participants.Remove(user);

            _context.Employees.Update(user);
            _context.Meetings.Update(meeting);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        // GET: Meetings/Details/5
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.room)
                .Include(m=>m.Participants)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        [Authorize(Roles = "NORMAL")]
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "Name");
            ViewBag.AllEmployees = _context.Employees.ToList();
            return View();
        }
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.room)
                .Include(m => m.Participants)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null)
            {
                return NotFound();
            }

            var participantIds = meeting.Participants.Select(p => p.EmployeeId);

            ViewBag.EmployeesNotParticipating = await _context.Employees
                .Where(employee => !participantIds.Contains(employee.EmployeeId))
                .ToListAsync();

            return View(meeting);
        }
        [Authorize(Roles = "NORMAL")]
        // POST: Meetings/Add/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int[] selectedEmployees)
        {
            var meeting = await _context.Meetings
                .Include(m => m.Participants)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null)
            {
                return NotFound();
            }

            if (selectedEmployees != null && selectedEmployees.Any())
            {
                // Pobierz pracowników na podstawie wybranych identyfikatorów
                var employeesToAdd = await _context.Employees
                    .Where(e => selectedEmployees.Contains(e.EmployeeId))
                    .ToListAsync();

                // Dodaj wybranych pracowników do spotkania
                meeting.Participants.AddRange(employeesToAdd);

                // Zapisz zmiany w bazie danych
                await _context.SaveChangesAsync();
            }

            // Przekieruj użytkownika do widoku szczegółów spotkania
            return RedirectToAction(nameof(Details), new { id = id });
        }

        // POST: Meetings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "NORMAL")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MeetingId,RoomId,Title,Description,StartingTime,EndingTime")] Meeting meeting, int[] selectedEmployees)
        {
            Room room = _context.Rooms.First(r => r.RoomId == meeting.RoomId);
            meeting.room = room;

            if (selectedEmployees != null && selectedEmployees.Any())
            {
                meeting.Participants = _context.Employees.Where(e => selectedEmployees.Contains(e.EmployeeId)).ToList();
                foreach(var employee in meeting.Participants)
                {
                    employee.Meetings.Add(meeting);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(meeting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", meeting.RoomId);
            ViewBag.AllEmployees = _context.Employees.ToList();

            return View(meeting);
        }
        // GET: Meetings/Edit/5
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Edit(int? id)
        {            
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.room)
                .Include(m => m.Participants)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null)
            {
                return NotFound();
            }
            var rooms = _context.Rooms;
            ViewBag.AllEmployees = _context.Employees.ToList();
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomId");
            return View(meeting);
        }
        [Authorize(Roles = "NORMAL")]
        // POST: Meetings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MeetingId,RoomId,Title,Description,StartingTime,EndingTime")] Meeting meeting, int[] selectedEmployees)
        {
            if (id != meeting.MeetingId)
            {
                return NotFound();
            }

            // Pobierz istniejące spotkanie z bazy danych wraz z uczestnikami
            var existingMeeting = await _context.Meetings
                .Include(m => m.Participants)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (existingMeeting == null)
            {
                return NotFound();
            }

            // Zaktualizuj pozostałe atrybuty spotkania
            existingMeeting.RoomId = meeting.RoomId;
            existingMeeting.Title = meeting.Title;
            existingMeeting.Description = meeting.Description;
            existingMeeting.StartingTime = meeting.StartingTime;
            existingMeeting.EndingTime = meeting.EndingTime;

            // Obsłuż dodawanie i usuwanie uczestników
            if (selectedEmployees != null && selectedEmployees.Any())
            {
                var employeesToAdd = _context.Employees.Where(e => selectedEmployees.Contains(e.EmployeeId)).ToList();
                existingMeeting.Participants.AddRange(employeesToAdd);

                foreach (var employee in employeesToAdd)
                {
                    // Dodaj spotkanie do listy spotkań pracownika
                    employee.Meetings.Add(existingMeeting);
                }

                var employeesToRemove = existingMeeting.Participants.Where(p => !selectedEmployees.Contains(p.EmployeeId)).ToList();
                foreach (var employee in employeesToRemove)
                {
                    existingMeeting.Participants.Remove(employee);
                    employee.Meetings.Remove(existingMeeting);
                }
            }
            else
            {
                // W przypadku, gdy nie wybrano żadnych pracowników, usuń wszystkich uczestników
                existingMeeting.Participants.Clear();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(existingMeeting);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MeetingExists(meeting.MeetingId))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", meeting.RoomId);
            return View(meeting);
        }

        [Authorize(Roles = "HR")]
        // GET: Meetings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _context.Meetings
                .Include(m => m.room)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }
        [Authorize(Roles = "HR")]
        // POST: Meetings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var meeting = await _context.Meetings.FindAsync(id);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MeetingExists(int id)
        {
            return _context.Meetings.Any(e => e.MeetingId == id);
        }
    }
}
