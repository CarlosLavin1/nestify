using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;
using mvcNestify;
using Microsoft.AspNetCore.Authorization;

namespace mvcNestify.Controllers
{
    public class AgentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Agents
        public async Task<IActionResult> Index()
        {
              return _context.Agents != null ? 
                          View(await _context.Agents.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Agents'  is null.");
        }

        // GET: Agents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.AgentID == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // GET: Agents/Create
        [Authorize]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Agents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AgentID,AgentSIN,FirstName,LastName,MiddleName,DateOfBirth,HomePhone,CellPhone,OfficePhone,OfficeEmail,StreetAddress,Municipality,Province,PostalCode,Username,AuthorizationLevel,CreatorID")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                if (ValidationHelper.GetAge(agent.DateOfBirth) >= 18)
                {
                    if(!ExistingSin(agent.AgentSIN)) 
                    {
                        if (!ExistingUsername(agent.Username))
                        {
                            _context.Add(agent);
                            await _context.SaveChangesAsync();
                            return RedirectToAction("Index");
                        }
                        ModelState.AddModelError("Username", "Agent with that username already exists");
                        return View(agent);
                    }
                    ModelState.AddModelError("AgentSIN", "Agent with that S.I.N number already exists");
                    return View(agent);
                }
                ModelState.AddModelError("DateOfBirth", "Invalid age, must be 18+");
                return View(agent);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Agents/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents.FindAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return View(agent);
        }

        // POST: Agents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AgentID,AgentSIN,FirstName,LastName,MiddleName,DateOfBirth,HomePhone,CellPhone,OfficePhone,OfficeEmail,StreetAddress,Municipality,Province,PostalCode,Username,AuthorizationLevel,CreatorID")] Agent agent)
        {
            if (id != agent.AgentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(agent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AgentExists(agent.AgentID))
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
            return View(agent);
        }

        // GET: Agents/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Agents == null)
            {
                return NotFound();
            }

            var agent = await _context.Agents
                .FirstOrDefaultAsync(m => m.AgentID == id);
            if (agent == null)
            {
                return NotFound();
            }

            return View(agent);
        }

        // POST: Agents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Agents == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Agents'  is null.");
            }
            var agent = await _context.Agents.FindAsync(id);
            if (agent != null)
            {
                _context.Agents.Remove(agent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AgentExists(int id)
        {
          return (_context.Agents?.Any(e => e.AgentID == id)).GetValueOrDefault();
        }

        public bool ExistingSin(string agentSin)
        {
            return (_context.Agents?.Any(e => e.AgentSIN == agentSin)).GetValueOrDefault();
        }

        public bool ExistingUsername(string username) 
        {
            return (_context.Agents?.Any(e => e.Username == username)).GetValueOrDefault();
        }
    }
}
