﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;

namespace mvcNestify.Controllers
{

    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ListingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Select(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            ICollection<Listing>? listings = _context.Listings
                .Where(listing => listing.CustomerID == id)
                .ToList();



            ICollection<ListingViewModel>? listingsOverivews =
                listings.Select(listing =>
                new ListingViewModel
                {
                    ListingID = listing.ListingID,
                    StreetAddress = listing.StreetAddress,
                    Municipality = listing.Municipality,
                    Province = listing.Province,
                    AgentID = listing.AgentID,
                    StartDate = listing.StartDate,
                    EndDate = listing.EndDate,
                    CustomerID = listing.CustomerID
                }).ToList();
            if (TempData["ListingSaved"] != null)
            {
                ViewBag.ListingSaved = TempData["ListingSaved"].ToString();
            }

            TempData["CustomerID"] = id;
            if (listingsOverivews == null) 
            {
                return View();
            }

            return View(listingsOverivews);

        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.Customer)
                .Include(l => l.ListingAgent)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        public async Task<IActionResult> CustDetails(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.Customer)
                .Include(l => l.ListingAgent)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // GET: Listings/Create
        [Authorize]
        public IActionResult Create(int? id)
        {

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName");
            List<SelectListItem> provinceOptions = new List<SelectListItem>()
            {
                new SelectListItem{Text = "-- SELECT A VALUE --", Value= "", Disabled = true, Selected = true },
                new SelectListItem{Text = "AB", Value= "Alberta" },
                new SelectListItem{Text = "BC", Value= "British Columbia" },
                new SelectListItem{Text = "MB", Value= "Manitoba" },
                new SelectListItem{Text = "NB", Value= "New Brunswick" },
                new SelectListItem{Text = "NL", Value= "Newfoundland and Labrador" },
                new SelectListItem{Text = "NT", Value= "Northwest Territories" },
                new SelectListItem{Text = "NS", Value= "Nova Scotia" },
                new SelectListItem{Text = "NU", Value= "Nunavut" },
                new SelectListItem{Text = "ON", Value= "Ontario" },
                new SelectListItem{Text = "PEI", Value= "Prince Edward Island" },
                new SelectListItem{Text = "QUE", Value= "Quebec" },
                new SelectListItem{Text = "SK", Value= "Saskatchewan" },
                new SelectListItem{Text = "YT", Value= "Yukon" }

            };
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", id);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingID,StreetAddress,Municipality,Province,PostalCode,CityLocation,Footage,NumOfBaths,NumOfRooms,NumOfStories,TypeOfHeating,Features,SpecialFeatures,SalesPrice,ContractSigned,AgentID,CustomerID,StartDate,EndDate,ListingStatus")] Listing listing)
        {
            List<SelectListItem> provinceOptions = new List<SelectListItem>()
            {
                new SelectListItem{Text = "-- SELECT A VALUE --", Value= "", Disabled = true, Selected = true },
                new SelectListItem{Text = "AB", Value= "Alberta" },
                new SelectListItem{Text = "BC", Value= "British Columbia" },
                new SelectListItem{Text = "MB", Value= "Manitoba" },
                new SelectListItem{Text = "NB", Value= "New Brunswick" },
                new SelectListItem{Text = "NL", Value= "Newfoundland and Labrador" },
                new SelectListItem{Text = "NT", Value= "Northwest Territories" },
                new SelectListItem{Text = "NS", Value= "Nova Scotia" },
                new SelectListItem{Text = "NU", Value= "Nunavut" },
                new SelectListItem{Text = "ON", Value= "Ontario" },
                new SelectListItem{Text = "PEI", Value= "Prince Edward Island" },
                new SelectListItem{Text = "QUE", Value= "Quebec" },
                new SelectListItem{Text = "SK", Value= "Saskatchewan" },
                new SelectListItem{Text = "YT", Value= "Yukon" }

            };

            listing.EndDate = listing.StartDate.AddMonths(3);
            if (!!listing.ContractSigned)
            {
                listing.ListingStatus = "Avaliable";
            }
            else
            {
                listing.ListingStatus = "Not Avaliable";
                listing.StartDate = DateTime.MinValue;
                listing.EndDate = DateTime.MinValue;
                listing.AgentID = null;
            }


            if (ModelState.IsValid)
            {
                var customer = _context.Customers.FirstOrDefault(cust => cust.CustomerID == listing.CustomerID);
                var agent = _context.Agents.FirstOrDefault(a => a.AgentID == listing.AgentID);
                if (customer.IsVerified != true) 
                {
                    ModelState.AddModelError("CustomerID", "Customer is not verifed, please wait for verification and try again.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
                    ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                    return View(listing);
                }
                if (agent.IsVerified == false)
                {

                    ModelState.AddModelError("AgentID", "Agent is not verifed, please wait for verification and try again.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
                    ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                    return View(listing);
                }
                if (ListingAddressExists(listing.Address)) 
                {
                    ModelState.AddModelError("StreetAddress", $"Listing at {listing.Address} already exists.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
                    ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                    return View(listing);
                }
                _context.Add(listing);
                await _context.SaveChangesAsync();
                TempData["ListingSaved"] = "Listing has been saved!";
                return RedirectToAction("Select", new { id = listing.CustomerID });
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            return View(listing);
        }

        // GET: Listings/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            List<SelectListItem> provinceOptions = new List<SelectListItem>()
            {
                new SelectListItem{Text = "-- SELECT A VALUE --", Value= "", Disabled = true, Selected = true },
                new SelectListItem{Text = "AB", Value= "Alberta" },
                new SelectListItem{Text = "BC", Value= "British Columbia" },
                new SelectListItem{Text = "MB", Value= "Manitoba" },
                new SelectListItem{Text = "NB", Value= "New Brunswick" },
                new SelectListItem{Text = "NL", Value= "Newfoundland and Labrador" },
                new SelectListItem{Text = "NT", Value= "Northwest Territories" },
                new SelectListItem{Text = "NS", Value= "Nova Scotia" },
                new SelectListItem{Text = "NU", Value= "Nunavut" },
                new SelectListItem{Text = "ON", Value= "Ontario" },
                new SelectListItem{Text = "PEI", Value= "Prince Edward Island" },
                new SelectListItem{Text = "QUE", Value= "Quebec" },
                new SelectListItem{Text = "SK", Value= "Saskatchewan" },
                new SelectListItem{Text = "YT", Value= "Yukon" }

            };
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");

            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            if (listing == null)
            {
                return NotFound();
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
            return View(listing);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ListingID,StreetAddress,Municipality,Province,PostalCode,CityLocation,Footage,NumOfBaths,NumOfRooms,NumOfStories,TypeOfHeating,Features,SpecialFeatures,SalesPrice,ContractSigned,AgentID,CustomerID,StartDate,EndDate,ListingStatus")] Listing listing)
        {
            List<SelectListItem> provinceOptions = new List<SelectListItem>()
            {
                new SelectListItem{Text = "-- SELECT A VALUE --", Value= "", Disabled = true, Selected = true },
                new SelectListItem{Text = "AB", Value= "Alberta" },
                new SelectListItem{Text = "BC", Value= "British Columbia" },
                new SelectListItem{Text = "MB", Value= "Manitoba" },
                new SelectListItem{Text = "NB", Value= "New Brunswick" },
                new SelectListItem{Text = "NL", Value= "Newfoundland and Labrador" },
                new SelectListItem{Text = "NT", Value= "Northwest Territories" },
                new SelectListItem{Text = "NS", Value= "Nova Scotia" },
                new SelectListItem{Text = "NU", Value= "Nunavut" },
                new SelectListItem{Text = "ON", Value= "Ontario" },
                new SelectListItem{Text = "PEI", Value= "Prince Edward Island" },
                new SelectListItem{Text = "QUE", Value= "Quebec" },
                new SelectListItem{Text = "SK", Value= "Saskatchewan" },
                new SelectListItem{Text = "YT", Value= "Yukon" }

            };

            if (id != listing.ListingID)
            {
                return NotFound();
            }

            listing.EndDate = listing.StartDate.AddMonths(3);
            if (!!listing.ContractSigned)
            {
                listing.ListingStatus = "Avaliable";
            }
            else
            {
                listing.ListingStatus = "Not Avaliable";
                listing.AgentID = null;
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ListingExists(listing.ListingID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["ListingSaved"] = "Listing has been updated!";
                return RedirectToAction("Select", new { id = listing.CustomerID });
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            return View(listing);
        }

        // GET: Listings/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .Include(l => l.Customer)
                .Include(l => l.ListingAgent)
                .FirstOrDefaultAsync(m => m.ListingID == id);
            if (listing == null)
            {
                return NotFound();
            }

            return View(listing);
        }

        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Listings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Listings'  is null.");
            }
            var listing = await _context.Listings.FindAsync(id);
            if (listing != null)
            {
                _context.Listings.Remove(listing);
            }

            await _context.SaveChangesAsync();
            TempData["ListingSaved"] = "Listing has been deleted!";
            return RedirectToAction("Select", new { id = listing.CustomerID });
        }

        private bool ListingExists(int id)
        {
            return (_context.Listings?.Any(e => e.ListingID == id)).GetValueOrDefault();
        }
        private bool ListingAddressExists(string address)
        {
            return (_context.Listings?.Any(e => 
                e.StreetAddress + 
                e.Municipality + 
                e.Province 
                == address ))
                .GetValueOrDefault();
        }
    }
}
