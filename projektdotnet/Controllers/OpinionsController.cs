using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using projektdotnet.Data;
using projektdotnet.Models;
using projektdotnet.Services;

namespace projektdotnet.Controllers
{
    public class OpinionsController : Controller
    {
        private readonly NewDbContext _context;
        private readonly EmployeeService _employeeService;

        public OpinionsController(NewDbContext context,EmployeeService employeeService)
        {
            _context = context;
            _employeeService = employeeService;
        }

        [Authorize(Roles = "HR")]
        // GET: Opinions
        public async Task<IActionResult> Index()
        {
            var newDbContext = await _context.Opinions.Include(o => o.Employee).ToListAsync();
            return View(newDbContext);
        }
        [Authorize(Roles = "HR")]
        // GET: Opinions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(m => m.OpinionId == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }
        [Authorize(Roles = "NORMAL")]
        // GET: Opinions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Opinions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "NORMAL")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OpinionId,EmployeeId,Title,Description")] Opinion opinion,bool anonymous)
        {
            if(anonymous)
            {
                opinion.EmployeeId = null;
                opinion.Employee = null;
                if (ModelState.IsValid)
                {
                    _context.Add(opinion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index","Home");
                }
            }
            var user = await _employeeService.GetEmployeeFromHttp();
            opinion.EmployeeId = user.EmployeeId;
            if (ModelState.IsValid)
            {
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Index","Home");
        }

        // GET: Opinions/Edit/5
        [Authorize(Roles = "NORMAL")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion == null)
            {
                return NotFound();
            }
            var AllEmployees = _context.Employees;
            ViewData["allEmployees"] = new SelectList(AllEmployees, "EmployeeId", "Login");
            return View(opinion);
        }

        // POST: Opinions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "NORMAL")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OpinionId,EmployeeId,Title,Description")] Opinion opinion)
        {
            if (id != opinion.OpinionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opinion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpinionExists(opinion.OpinionId))
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "Discriminator", opinion.EmployeeId);
            return View(opinion);
        }

        // GET: Opinions/Delete/5
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opinion = await _context.Opinions
                .Include(o => o.Employee)
                .FirstOrDefaultAsync(m => m.OpinionId == id);
            if (opinion == null)
            {
                return NotFound();
            }

            return View(opinion);
        }
        [Authorize(Roles = "HR")]
        // POST: Opinions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion != null)
            {
                _context.Opinions.Remove(opinion);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpinionExists(int id)
        {
            return _context.Opinions.Any(e => e.OpinionId == id);
        }
    }
}
