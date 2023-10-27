using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;

namespace mvcNestify.Controllers
{
    public class ShowingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Showings
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Showings.Include(s => s.Customer).Include(s => s.Listing);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Showings/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings
                .Include(s => s.Customer)
                .Include(s => s.Listing)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (showing == null)
            {
                return NotFound();
            }

            return View(showing);
        }

        // GET: Showings/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FirstName");
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "CityLocation");
            return View();
        }

        // POST: Showings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,ListingID,Date,StartTime,EndTime,Comments")] Showing showing)
        {
            if (ModelState.IsValid)
            {
                _context.Add(showing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FirstName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "CityLocation", showing.ListingID);
            return View(showing);
        }

        // GET: Showings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings.FindAsync(id);
            if (showing == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FirstName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "CityLocation", showing.ListingID);
            return View(showing);
        }

        // POST: Showings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("CustomerID,ListingID,Date,StartTime,EndTime,Comments")] Showing showing)
        {
            if (id != showing.ListingID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(showing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowingExists(showing.ListingID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FirstName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "CityLocation", showing.ListingID);
            return View(showing);
        }

        // GET: Showings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings
                .Include(s => s.Customer)
                .Include(s => s.Listing)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (showing == null)
            {
                return NotFound();
            }

            return View(showing);
        }

        // POST: Showings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Showings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Showings'  is null.");
            }
            var showing = await _context.Showings.FindAsync(id);
            if (showing != null)
            {
                _context.Showings.Remove(showing);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShowingExists(int? id)
        {
          return (_context.Showings?.Any(e => e.ListingID == id)).GetValueOrDefault();
        }
    }
}
