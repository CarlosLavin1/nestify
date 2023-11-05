using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;

namespace mvcNestify.Controllers
{
    [Authorize]
    public class ShowingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShowingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Showings
        public async Task<IActionResult> Index(string searchCriteria)
        {
            ViewData["CurrentFiler"] = searchCriteria;
            var showingList =
                from showing in _context.Showings
                select showing;


            if (!String.IsNullOrEmpty(searchCriteria)) 
            {
                if(DateTime.TryParse(searchCriteria, out DateTime s))
                {
                    showingList = showingList.Where(s =>
                        s.Date.Date.Equals(Convert.ToDateTime(s).Date));
                }
                showingList = showingList.Where(s =>
                s.Agent.FirstName.Contains(searchCriteria) ||
                s.Agent.LastName.Contains(searchCriteria) ||
                s.Agent.MiddleName.Contains(searchCriteria));
            }
            if (showingList.IsNullOrEmpty()) 
            {
                ViewBag.NoShowing = $"There were no records that matched your search {searchCriteria} in the system. Please try again.";
            }

            return View(await showingList.ToListAsync());
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
                .Include(s => s.Agent)
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
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName");
            ViewData["ListingID"] = new SelectList(_context.Listings.Where(l => l.ListingStatus.Trim().StartsWith("Av")), "ListingID", "Address");
            return View();
        }

        // POST: Showings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerID,ListingID,AgentID,Customer,Listing,Date,StartTime,EndTime,Comments")] Showing showing)
        {
            List<Listing> availableListings = _context.Listings.Where(l => l.ListingStatus.Trim().StartsWith("Av")).ToList();
            showing.Customer = _context.Customers.First(c => c.CustomerID == showing.CustomerID);
            showing.Listing = _context.Listings.First(c => c.ListingID == showing.ListingID);
            showing.Agent = _context.Agents.First(c => c.AgentID == showing.AgentID);

            if (ModelState.IsValid)
            {
                if (TimeSlotIsTaken(showing))
                {
                    ModelState.AddModelError("", "Time slot is not available");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
                    ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
                    return View(showing);
                }
                if (!CustomerDoesNotOwn(showing)) 
                {
                    ModelState.AddModelError("CustomerID", "Customer cannot book a showing at an owned listing.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
                    ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
                    return View(showing);
                }
                if (AgentNotAvailable(showing))
                {
                    ModelState.AddModelError("AgentID", "Agent is not avaliable for a showing at this date and time.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
                    ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
                    return View(showing);
                }
                _context.Add(showing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
            ViewData["ListingID"] = new SelectList(availableListings, "ListingID", "Address", showing.ListingID);
            return View(showing);
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
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
            ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "Address", showing.ListingID);
            return View(showing);
        }

        // POST: Showings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? listingID, int? customerID, [Bind("CustomerID,ListingID,Date,StartTime,EndTime,AgentId,Comments")] Showing showing)
        {
            if (listingID != showing.ListingID && customerID != showing.CustomerID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (TimeSlotIsTaken(showing))
                    {
                        ModelState.AddModelError("", "Time slot is not available");
                        ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                        ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
                        ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "Address", showing.ListingID);
                        return View(showing);
                    }
                    if (AgentNotAvailable(showing))
                    {
                        ModelState.AddModelError("AgentID", "Agent is not avaliable for a showing at this date and time.");
                        ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", showing.CustomerID);
                        ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
                        ViewData["ListingID"] = new SelectList(_context.Listings, "ListingID", "Address", showing.ListingID);
                        return View(showing);
                    }

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
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", showing.AgentID);
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
        public async Task<IActionResult> DeleteConfirmed(int? listingID, int? customerID)
        {
            if (_context.Showings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Showings'  is null.");
            }
            var showing = await _context.Showings.FindAsync(listingID, customerID);
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

        private bool CustomerDoesNotOwn(Showing showing)
        {
            var listing = _context.Listings.FirstOrDefault(l => l.ListingID == showing.ListingID);

            if (listing.CustomerID == showing.CustomerID)
            {
                return false;
            }

            return true;
        }
        private bool TimeSlotIsTaken(Showing showing)
        {
            List<Showing> showingsAtListing = _context.Showings.Where(s => s.ListingID == showing.ListingID).ToList();
            //check if showing times overlap with any existing showings
            return showingsAtListing.Any(s => (s.StartTime <= showing.StartTime && s.EndTime >= showing.StartTime) ||
                (s.EndTime >= showing.EndTime && s.StartTime <= showing.EndTime));
        }

        private bool AgentNotAvailable(Showing showing)
        {
            List<Showing> showingsWithAgent = _context.Showings.Where(s => s.AgentID == showing.AgentID).ToList();

            return showingsWithAgent.Any(s => (s.StartTime <= showing.StartTime && s.EndTime >= showing.StartTime) ||
                (s.EndTime >= showing.EndTime && s.StartTime <= showing.EndTime));
        }
    }
}
