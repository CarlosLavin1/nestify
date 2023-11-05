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
        public async Task<IActionResult> Details(int? customerID, int? listingID)
        {
            if (customerID == null || listingID == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings
                .Include(s => s.Customer)
                .Include(s => s.Listing)
                .FirstOrDefaultAsync(m => m.ListingID == listingID && m.CustomerID == customerID);
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName");
            ViewData["ListingID"] = new SelectList(_context.Listings.Where(l => l.ListingStatus.Trim().StartsWith("Av")), "ListingID", "Address");
            return View();
        }

        // POST: Showings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,ListingID,Customer,Listing,Date,StartTime,EndTime,Comments")] Showing showing)
        {
            List<Listing> availableListings = _context.Listings.Where(l => l.ListingStatus.Trim().StartsWith("Av")).ToList();
            showing.Customer = _context.Customers.First(c => c.CustomerID == showing.CustomerID);
            showing.Listing = _context.Listings.First(c => c.ListingID == showing.ListingID);

            if (ModelState.IsValid)
            {
                if (TimeSlotIsTaken(showing))
                {
                    ModelState.AddModelError("", "Time slot is not available");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                    ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
                    return View(showing);
                }
                _context.Add(showing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
            return View(showing);
        }
        private bool TimeSlotIsTaken(Showing showing)
        {
            List<Showing> showingsAtListing = _context.Showings.Where(s => s.ListingID == showing.ListingID).ToList();
            //check if showing times overlap with any existing showings
            return showingsAtListing.Any(s => (s.StartTime <= showing.StartTime && s.EndTime >= showing.StartTime) ||
                (s.EndTime >= showing.EndTime && s.StartTime <= showing.EndTime));
        }

        // GET: Showings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? customerID, int? listingID)
        {
            if (customerID == null || listingID == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings.FindAsync(listingID, customerID);
            if (showing == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "Address", showing.ListingID);
            return View(showing);
        }

        // POST: Showings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? listingID, int? customerID, [Bind("CustomerID,ListingID,Date,StartTime,EndTime,Comments")] Showing showing)
        {
            if (listingID != showing.ListingID && customerID != showing.CustomerID)
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
                    if (!ShowingExists(showing.ListingID, showing.CustomerID))
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
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "Address", showing.ListingID);
            return View(showing);
        }

        // GET: Showings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? customerID, int? listingID)
        {
            if (customerID == null || listingID == null || _context.Showings == null)
            {
                return NotFound();
            }

            var showing = await _context.Showings
                .Include(s => s.Customer)
                .Include(s => s.Listing)
                .FirstOrDefaultAsync(m => m.ListingID == listingID && m.CustomerID == customerID);
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

        private bool ShowingExists(int? listingID, int? customerID)
        {
            return (_context.Showings?.Any(e => e.ListingID == listingID && e.CustomerID == customerID)).GetValueOrDefault();
        }
    }
}
