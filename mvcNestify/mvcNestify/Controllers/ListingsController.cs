using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;
using static mvcNestify.EmailServices.EmailSender;

namespace mvcNestify.Controllers
{

    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        public ListingsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Listings
                .Include(c => c.Contract)
                .Where(l => l.ListingStatus != "Not Avaliable");
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Select(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            ICollection<Listing>? listings = _context.Listings
                .Include(l => l.Contract)
                .Include(cust => cust.Customer)
                .Where(c => c.CustomerID == id)
                .ToList();


            ICollection<ListingViewModel>? model =
                         listings.Select(listing => {
                             ICollection<Models.Contract> contract = listing.Contract;
                             if(listing.ContractSigned == true) 
                             {
                                 return new ListingViewModel
                                 {
                                     ListingID = listing.ListingID,
                                     StreetAddress = listing.StreetAddress,
                                     Municipality = listing.Municipality,
                                     Province = listing.Province,
                                     AgentID = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).AgentID,
                                     StartDate = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).StartDate,
                                     EndDate = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).EndDate.Date,
                                     CustomerID = listing.CustomerID,
                                     CustFirstName = listing.Customer.FirstName,
                                     CustMiddleName = listing.Customer.MiddleName,
                                     CustLastName = listing.Customer.LastName
                                 };
                             }
                             return new ListingViewModel
                             {
                                 ListingID = listing.ListingID,
                                 StreetAddress = listing.StreetAddress,
                                 Municipality = listing.Municipality,
                                 Province = listing.Province,
                                 CustomerID = listing.CustomerID,
                                 CustFirstName = listing.Customer.FirstName,
                                 CustMiddleName = listing.Customer.MiddleName,
                                 CustLastName = listing.Customer.LastName
                             };

                         }).ToList();

            if (TempData["ListingSaved"] != null)
            {
                ViewBag.ListingSaved = TempData["ListingSaved"].ToString();
            }

            TempData["CustomerID"] = id;


            if (model == null)
            {
                return View();
            }

            return View(model);

        }

        // GET: Listings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
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
        public async Task<IActionResult> Create(ContractViewModel contractModel)
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

            Models.Contract contract = new()
            {
                StartDate = contractModel.StartDate,
                SalesPrice = contractModel.SalesPrice,
                AgentID = contractModel.AgentID,
                ListingID = 0
            };
            Listing listing = new()
            {
                StreetAddress = contractModel.StreetAddress,
                Municipality = contractModel.Municipality,
                CityLocation = contractModel.CityLocation,
                Province = contractModel.Province,
                PostalCode = contractModel.PostalCode,
                Footage = contractModel.Footage,
                NumOfBaths = contractModel.NumOfBaths,
                NumOfRooms = contractModel.NumOfRooms,
                NumOfStories = contractModel.NumOfStories,
                TypeOfHeating = contractModel.TypeOfHeating,
                Features = contractModel.Features,
                SpecialFeatures = contractModel.SpecialFeatures,
                ListingStatus = " ",
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID
            };

            if (ModelState.IsValid)
            {

                var customer = _context.Customers.FirstOrDefault(cust => cust.CustomerID == listing.CustomerID);
                var agent = _context.Agents.FirstOrDefault(a => a.AgentID == contract.AgentID);
                if (contractModel.AgentID != null) 
                {
                    if (agent.IsVerified == false)
                    {

                        ModelState.AddModelError("AgentID", "Agent is not verifed, please wait for verification and try again.");
                        ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                        ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
                        ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                        return View(contractModel);
                    }
                }
                if (customer.IsVerified != true)
                {
                    ModelState.AddModelError("CustomerID", "Customer is not verifed, please wait for verification and try again.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
                    ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                    return View(contractModel);
                }
                if (ListingAddressExists(listing.Address))
                {
                    ModelState.AddModelError("StreetAddress", $"Listing at {listing.Address} already exists.");
                    ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
                    ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
                    ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
                    return View(contractModel);
                }
                if (!!listing.ContractSigned)
                {
                    listing.ListingStatus = "Avaliable";
                    contract.EndDate = contract.StartDate.AddMonths(3);
                }
                else
                {
                    listing.ListingStatus = "Not Avaliable";
                }

                _context.Add(listing);
                await _context.SaveChangesAsync();

                string url = Url.Action("CustDetails", "Listings", new { id = listing.ListingID }, protocol: "https");
                var message = new EmailMessage(new string[] { listing.Customer.Email },
                    "Listing Creation",
                    $"Your listing has been created! Your listing number is {listing.ListingID}. \n The link to view your listing is {url}. \n" +
                    $"Thank you, \n" +
                    $"From the Nestify Staff");

                if (listing.ListingStatus == "Avaliable")
                {
                    contract.ListingID = listing.ListingID;
                    _context.Contracts.Add(contract);
                    await _context.SaveChangesAsync();
                }

                TempData["ListingSaved"] = "Listing has been saved!";
                return RedirectToAction("Select", new { id = listing.CustomerID });
            }
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            return View(contractModel);
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
            var contract = await _context.Contracts.FindAsync();

            if (listing == null)
            {
                return NotFound();
            }

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
            return View(listing);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractViewModel contractModel)
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

            Listing listing = new()
            {
                ListingID = (int)contractModel.ListingID,
                StreetAddress = contractModel.StreetAddress,
                Municipality = contractModel.Municipality,
                CityLocation = contractModel.CityLocation,
                Province = contractModel.Province,
                PostalCode = contractModel.PostalCode,
                Footage = contractModel.Footage,
                NumOfBaths = contractModel.NumOfBaths,
                NumOfRooms = contractModel.NumOfRooms,
                NumOfStories = contractModel.NumOfStories,
                TypeOfHeating = contractModel.TypeOfHeating,
                Features = contractModel.Features,
                SpecialFeatures = contractModel.SpecialFeatures,
                ListingStatus = null,
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID
            };

            Models.Contract contract = new()
            {
                StartDate = contractModel.StartDate,
                SalesPrice = contractModel.SalesPrice,
                AgentID = contractModel.AgentID,
            };

            if (id != listing.ListingID)
            {
                return NotFound();
            }

            //listing.EndDate = listing.StartDate.AddMonths(3);
            if (!!listing.ContractSigned)
            {
                listing.ListingStatus = "Avaliable";
            }
            else
            {
                listing.ListingStatus = "Not Avaliable";
                //listing.AgentID = null;
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
                //return RedirectToAction("Select", new { id = listing.CustomerID });
            }
            //ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            //ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", listing.AgentID);
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
                if (listing.Contract.Count == 0)
                {
                    if (listing.Showing.Count == 0)
                    {
                        _context.Listings.Remove(listing);
                        await _context.SaveChangesAsync();
                        TempData["ListingSaved"] = "Listing has been deleted!";
                        return RedirectToAction("Select", new { id = listing.CustomerID });
                    }
                    ModelState.AddModelError("Showing", $"Listing at {listing.Address} still has scheduled showings.");
                }
                ModelState.AddModelError("Contract", $"Listing at {listing.Address} still has an active contract.");
            }

            return View();
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
                == address))
                .GetValueOrDefault();
        }
    }
}
