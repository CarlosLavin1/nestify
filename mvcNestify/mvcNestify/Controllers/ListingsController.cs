using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;
using NuGet.DependencyResolver;
using NuGet.Versioning;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using static mvcNestify.EmailServices.EmailSender;

namespace mvcNestify.Controllers
{



    public class ListingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;

        List<string> specialFeats = new()
            {
                "Has a Fireplace",
                "Has a Baby Barn",
                "Has Central Air",
                "1 Bay Garage",
                "2 Bay Garage",
                "3 Bay Garage"
            };


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

        public ListingsController(ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Listings
                .Include(c => c.Contract)
                .Where(l => l.ListingStatus != "Not Available");
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

            ICollection<ListingViewModel>? model;

            if (listings.Count() > 0)
            {

                model =
                     listings.Select(listing =>
                     {
                         ICollection<Models.Contract> contract = listing.Contract;
                         if (listing.ContractSigned == true)
                         {
                             return new ListingViewModel
                             {
                                 ListingID = listing.ListingID,
                                 StreetAddress = listing.StreetAddress,
                                 Municipality = listing.Municipality,
                                 Province = listing.Province,
                                 AgentID = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).AgentID,
                                 StartDate = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).StartDate,
                                 EndDate = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).EndDate,
                                 CustomerID = listing.CustomerID,
                                 CustFirstName = listing.Customer.FirstName,
                                 CustMiddleName = listing.Customer.MiddleName,
                                 CustLastName = listing.Customer.LastName
                             };

                         }
                         else
                         {
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

                         }

                     }).ToList();
            }
            else
            {
                ICollection<Customer>? customer = _context.Customers
                .Where(c => c.CustomerID == id)
                .ToList();

                model =
                    customer.Select(cust =>
                    {
                        return new ListingViewModel
                        {
                            CustomerID = cust.CustomerID,
                            CustFirstName = cust.FirstName,
                            CustMiddleName = cust.MiddleName,
                            CustLastName = cust.LastName
                        };
                    }).ToList();
            }

            if (TempData["ListingSaved"] != null)
            {
                ViewBag.ListingSaved = TempData["ListingSaved"].ToString();
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
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(m => m.ListingID == id);

            AgentListingViewModel model = new();

            if (contract != null)
            {
                var agent = await _context.Agents
                    .FirstOrDefaultAsync(m => m.AgentID == contract.AgentID);
                model = new()
                {
                    AgentID = agent.AgentID,
                    AgentFirstName = agent.FirstName,
                    AgentMiddleName = agent.MiddleName,
                    AgentLastName = agent.LastName,
                    OfficeEmail = agent.OfficeEmail,
                    OfficePhone = agent.OfficePhone,
                    ListingID = listing.ListingID,
                    SalesPrice = contract.SalesPrice,
                    StreetAddress = listing.StreetAddress,
                    Municipality = listing.Municipality,
                    CityLocation = listing.CityLocation,
                    Province = listing.Province,
                    PostalCode = listing.PostalCode,
                    Footage = listing.Footage,
                    NumOfBaths = listing.NumOfBaths,
                    NumOfRooms = listing.NumOfRooms,
                    NumOfStories = listing.NumOfStories,
                    TypeOfHeating = listing.TypeOfHeating,
                    Features = listing.Features,
                    SpecialFeatures = listing.SpecialFeatures,
                    CustomerID = listing.CustomerID,
                    CustFirstName = listing.Customer.FirstName,
                    CustLastName = listing.Customer.LastName,
                    CustMiddleName = listing.Customer.MiddleName,
                    ListingStatus = listing.ListingStatus,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    ContractSigned = listing.ContractSigned
                };
            }
            else
            {
                model = new()
                {
                    ListingID = listing.ListingID,
                    StreetAddress = listing.StreetAddress,
                    Municipality = listing.Municipality,
                    CityLocation = listing.CityLocation,
                    Province = listing.Province,
                    PostalCode = listing.PostalCode,
                    Footage = listing.Footage,
                    NumOfBaths = listing.NumOfBaths,
                    NumOfRooms = listing.NumOfRooms,
                    NumOfStories = listing.NumOfStories,
                    TypeOfHeating = listing.TypeOfHeating,
                    Features = listing.Features,
                    SpecialFeatures = listing.SpecialFeatures,
                    CustomerID = listing.CustomerID,
                    CustFirstName = listing.Customer.FirstName,
                    CustLastName = listing.Customer.LastName,
                    CustMiddleName = listing.Customer.MiddleName,
                    ListingStatus = listing.ListingStatus,
                    ContractSigned = listing.ContractSigned
                };
            }

            if (listing == null)
            {
                return NotFound();
            }

            return View(model);
        }

        public async Task<IActionResult> CustDetails(int? id)
        {
            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings
                .FirstOrDefaultAsync(m => m.ListingID == id);
            var contract = await _context.Contracts
                .FirstOrDefaultAsync(m => m.ListingID == id);
            Agent? agent = new();
            if(listing.ContractSigned)
                agent = await _context.Agents
                    .FirstOrDefaultAsync(m => m.AgentID == contract.AgentID);

            AgentListingViewModel model = new()
            {
                AgentID = agent.AgentID,
                AgentFirstName = agent.FirstName,
                AgentMiddleName = agent.MiddleName,
                AgentLastName = agent.LastName,
                OfficeEmail = agent.OfficeEmail,
                OfficePhone = agent.OfficePhone,
                ListingID = listing.ListingID,
                SalesPrice = contract != null ? contract.SalesPrice : 0,
                StreetAddress = listing.StreetAddress,
                Municipality = listing.Municipality,
                CityLocation = listing.CityLocation,
                Province = listing.Province,
                PostalCode = listing.PostalCode,
                Footage = listing.Footage,
                NumOfBaths = listing.NumOfBaths,
                NumOfRooms = listing.NumOfRooms,
                NumOfStories = listing.NumOfStories,
                TypeOfHeating = listing.TypeOfHeating,
                Features = listing.Features,
                SpecialFeatures = listing.SpecialFeatures
            };

            if (listing == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // GET: Listings/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            var agents = _context.Agents.Where(a => a.IsVerified == true);
            var customers = _context.Agents.Where(c => c.IsVerified == true);

            ContractViewModel contract = new ContractViewModel();
            contract.Images = _context.Image.Where(i => i.IsListingImage).ToList();

            if (agents.Count() > 0)
            {
                ViewData["AgentID"] = new SelectList(agents, "AgentID", "FullName");
            }
            else
            {
                ModelState.AddModelError("AgentID", "No verifed agents in the system.");
            }

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", id);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            ViewData["SpecialFeatures"] = new MultiSelectList(specialFeats);
            return View(contract);
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractViewModel contractModel, List<string> SpecialFeatures)
        {
            Models.Contract contract = new();

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
                SpecialFeatures = "",
                ListingStatus = " ",
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID,
                Images = contractModel.Images
            };

            foreach (string feat in SpecialFeatures)
                listing.SpecialFeatures += $"{feat}. ";


            var customer = _context.Customers.FirstOrDefault(cust => cust.CustomerID == listing.CustomerID);

            if (!customer.IsVerified == true)
            {
                ModelState.AddModelError("CustomerID", "Customer is not verifed, please wait for verification and try again.");
                ModelState.ClearValidationState(nameof(contractModel));
            }

            if (ListingAddressExists(listing.Address))
            {
                ModelState.AddModelError("StreetAddress", $"Listing at {listing.Address} already exists.");
                ModelState.ClearValidationState(nameof(contractModel));

            }

            if (ModelState.IsValid)
            {

                if (listing.ContractSigned)
                {
                    contract = new()
                    {
                        StartDate = (DateTime)contractModel.StartDate,
                        SalesPrice = (decimal)contractModel.SalesPrice,
                        AgentID = contractModel.AgentID,
                        ListingID = 0
                    };

                    listing.ListingStatus = "Available";
                    contract.EndDate = contract.StartDate.AddMonths(3);
                }
                else
                {
                    listing.ListingStatus = "Not Available";
                }

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();


                string url = Url.Action("CustDetails", "Listings", new { id = listing.ListingID }, protocol: "https");
                var message = new EmailMessage(new string[] { listing.Customer.Email },
                    "Listing Creation",
                    $"Your listing has been created! Your listing number is {listing.ListingID}. \n The link to view your listing is {url}. \n" +
                    $"Thank you, \n" +
                    $"From the Nestify Staff");

                if (listing.ListingStatus == "Available")
                {
                    contract.ListingID = listing.ListingID;
                    _context.Contracts.Add(contract);
                    await _context.SaveChangesAsync();
                }

                TempData["ListingSaved"] = "Listing has been saved!";
                return RedirectToAction("Select", new { id = listing.CustomerID });

            }

            ViewData["SpecialFeatures"] = new MultiSelectList(specialFeats, listing.SpecialFeatures);
            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text", listing.Province);
            return View(contractModel);
        }

        // GET: Listings/Edit/5
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            ContractViewModel model = new();

            if (id == null || _context.Listings == null)
            {
                return NotFound();
            }

            var listing = await _context.Listings.FindAsync(id);
            var contract = _context.Contracts.Where(c => c.ListingID == id).FirstOrDefault();

            if (listing == null)
            {
                return NotFound();
            }

            model = new()
            {
                ListingID = listing.ListingID,
                CustomerID = listing.CustomerID,
                StreetAddress = listing.StreetAddress,
                Municipality = listing.Municipality,
                CityLocation = listing.CityLocation,
                Province = listing.Province,
                PostalCode = listing.PostalCode,
                Footage = listing.Footage,
                NumOfBaths = listing.NumOfBaths,
                NumOfRooms = listing.NumOfRooms,
                NumOfStories = listing.NumOfStories,
                TypeOfHeating = listing.TypeOfHeating,
                Features = listing.Features,
                SpecialFeatures = listing.SpecialFeatures,
                ListingStatus = listing.ListingStatus,
                ContractSigned = listing.ContractSigned,
                Images = listing.Images
            };

            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName");

            if (contract != null)
            {
                model.ContractID = contract.ContractID;
                model.StartDate = contract.StartDate;
                model.EndDate = contract.EndDate;
                model.SalesPrice = contract.SalesPrice;
                model.AgentID = contract.AgentID;

                ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
            }

            List<string> specialFeatures = model.SpecialFeatures.Split('.').Select(feat => feat.Trim()).ToList();

            ViewData["SpecialFeatures"] = new MultiSelectList(specialFeats, specialFeatures); ;
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text", listing.Province);
            ViewData["CustomerID"] = _context.Customers.FirstOrDefault(c => c.CustomerID == listing.CustomerID).FullName;


            return View(model);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractViewModel contractModel, List<string> SpecialFeatures)
        {
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
                SpecialFeatures = null,
                ListingStatus = null,
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID,
                Images = contractModel.Images,
            };


            foreach (string feat in SpecialFeatures)
                listing.SpecialFeatures += $"{feat}. ";


            Models.Contract contract = new();

            if (id != listing.ListingID)
            {
                return NotFound();
            }


            if (!!listing.ContractSigned)
            {
                contract = new()
                {
                    StartDate = (DateTime)contractModel.StartDate,
                    SalesPrice = (decimal)contractModel.SalesPrice,
                    EndDate  = (DateTime)contractModel.StartDate,
                    ListingID = listing.ListingID,
                    AgentID = contractModel.AgentID,
                };

                contract.EndDate = contract.EndDate.AddMonths(3);

                if (contractModel.ContractID != null)
                {
                    contract.ContractID = contractModel.ContractID;
                }

                listing.ListingStatus = "Available";

            }
            else
            {
                listing.ListingStatus = "Not Available";
                contract.AgentID = null;
            }

            var customer = _context.Customers.FirstOrDefault(cust => cust.CustomerID == listing.CustomerID);

            if (customer.IsVerified != true)
            {
                ModelState.AddModelError("CustomerID", "Customer is not verifed, please wait for verification and try again.");
            }

            if (ListingAddressExists(listing.Address))
            {
                ModelState.AddModelError("StreetAddress", $"Listing at {listing.Address} already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(listing);

                    await _context.SaveChangesAsync();

                    if (contract.ContractID != null)
                    {
                        _context.Update(contract);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        _context.Add(contract);
                        await _context.SaveChangesAsync();
                    }
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
            else
            {
                foreach (var key in ModelState.Keys)
                {
                    var error = ModelState[key].Errors.FirstOrDefault();
                    if (error != null)
                    {
                        // Log or handle the error as needed
                        // You can also use ViewData to pass errors to the view
                        ViewData["Error"] = error.ErrorMessage;
                    }
                }
            }

            List<string> specialFeatures = listing.SpecialFeatures.Split('.').Select(feat => feat.Trim()).ToList();

            ViewData["SpecialFeatures"] = new MultiSelectList(specialFeats, listing.SpecialFeatures);
            ViewData["CustomerID"] = _context.Customers.FirstOrDefault(c => c.CustomerID == listing.CustomerID).FullName;
            //ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents, "AgentID", "FullName", contract.AgentID);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            //view expects contractviewmodel
            return View(contractModel);
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
