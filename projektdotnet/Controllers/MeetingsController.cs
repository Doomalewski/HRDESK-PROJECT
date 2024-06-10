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
        private readonly EmployeeService _employeeService;
        private readonly MeetingService _meetingService;
        private readonly RoomService _roomService;
        public MeetingsController(EmployeeService employeeService, MeetingService meetingService,RoomService roomService)
        {
            _roomService = roomService;
            _employeeService = employeeService;
            _meetingService = meetingService;
        }

        // GET: Meetings
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Index()
        {
            var user = await _employeeService.GetEmployeeFromHttp();
            var newDbContext = await _meetingService.GetMeetingsWithParticipant(user.EmployeeId);
            return View(newDbContext);
        }
        [HttpPost]
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> LeaveMeeting(int id)
        {
            var meeting = await _meetingService.GetMeetingById(id);
            var user = await _employeeService.GetEmployeeFromHttp();

            user.Meetings.Remove(meeting);
            meeting.Participants.Remove(user);

            await _employeeService.UpdateEmployee(user);
            await _meetingService.UpdateMeeting(meeting);

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

            var meeting = await _meetingService.GetMeetingById(id);
            if (meeting == null)
            {
                return NotFound();
            }

            return View(meeting);
        }

        // GET: Meetings/Create
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Create()
        {
            var rooms = await _roomService.GetAllRooms();
            ViewData["RoomId"] = new SelectList(rooms , "RoomId", "Name");
            var AllEmployees = await _employeeService.GetAllEmployees();
            ViewBag.AllEmployees = AllEmployees;
            return View();
        }

        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Add(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meeting = await _meetingService.GetMeetingById(id);

            if (meeting == null)
            {
                return NotFound();
            }

            var participantIds = meeting.Participants.Select(p => p.EmployeeId);
            var employees = await _employeeService.GetAllEmployees();
            var employeesNotParticipating = employees
                .Where(employee => !participantIds.Contains(employee.EmployeeId));
            ViewBag.EmployeesNotParticipating = employeesNotParticipating;

            return View(meeting);
        }
        [Authorize(Roles = "NORMAL")]
        // POST: Meetings/Add/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int id, int[] selectedEmployees)
        {
            var meeting = await _meetingService.GetMeetingById(id);

            if (meeting == null)
            {
                return NotFound();
            }

            if (selectedEmployees != null && selectedEmployees.Any())
            {
                var employees = await _employeeService.GetAllEmployees();
                var employeesToAdd = employees
                    .Where(e => selectedEmployees.Contains(e.EmployeeId));

                meeting.Participants.AddRange(employeesToAdd);
                await _meetingService.UpdateMeeting(meeting);
            }
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
            Room room = await _roomService.GetRoomById(meeting.RoomId);
            meeting.room = room;

            if (selectedEmployees != null && selectedEmployees.Any())
            {
                var employees = await _employeeService.GetAllEmployees();
                meeting.Participants = employees.Where(e => selectedEmployees.Contains(e.EmployeeId)).ToList();
                foreach(var employee in meeting.Participants)
                {
                    employee.Meetings.Add(meeting);
                }
            }

            if (ModelState.IsValid)
            {
                await _meetingService.AddMeeting(meeting);
                return RedirectToAction(nameof(Index));
            }
            var rooms = await _roomService.GetAllRooms();
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomId", meeting.RoomId);
            ViewBag.AllEmployees = _employeeService.GetAllEmployees();

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

            var meeting = await _meetingService.GetMeetingById(id);
            if (meeting == null)
            {
                return NotFound();
            }
            var rooms = await _roomService.GetAllRooms();
            ViewBag.AllEmployees = _employeeService.GetAllEmployees();
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
            var existingMeeting = await _meetingService.GetMeetingById(id);

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
                var employees = await _employeeService.GetAllEmployees();
                var employeesToAdd = employees.Where(e => selectedEmployees.Contains(e.EmployeeId)).ToList();
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
                    await _meetingService.UpdateMeeting(existingMeeting);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _meetingService.MeetingExists(meeting.MeetingId))
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
            var rooms = await _roomService.GetAllRooms();
            ViewData["RoomId"] = new SelectList(rooms, "RoomId", "RoomId", meeting.RoomId);
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

            var meeting = await _meetingService.GetMeetingById(id);
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
            var meeting = await _meetingService.GetMeetingById(id);
            if (meeting != null)
            {
                await _meetingService.RemoveMeeting(meeting);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
