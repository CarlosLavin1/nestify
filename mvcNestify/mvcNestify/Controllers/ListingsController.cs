using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;
using System.Security.Permissions;
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
            "Garage"
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
                                 ListingAgent = contract.FirstOrDefault(c => c.ListingID == listing.ListingID).ListingAgent.FullName.ToString(),
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
                    SalesPrice = listing.SalesPrice,
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
                    SalesPrice = listing.SalesPrice,
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
            if (listing.ContractSigned)
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
                SalesPrice = listing.SalesPrice,
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

            // get all unused listing images

            List<ImageSelectionViewModel> imageList =
                _context.Image.Where(i => i.Validated && i.IsListingImage && i.ListingId == null).Select(i =>
                new ImageSelectionViewModel
                {
                    ImageID = i.ImageID,
                    Name = i.Name,
                    FileName = i.FileName,
                    Description = i.Description,
                    AltText = i.AltText,
                    UploadDateTime = i.UploadDateTime,
                    Validated = i.Validated,
                    StaffID = i.StaffID,
                    Listing = i.Listing,
                    ListingId = i.ListingId,
                    Agent = i.Agent,
                    AgentId = i.AgentId,
                    IsVisible = true,
                    IsAgentImage = i.IsAgentImage,
                    IsListingImage = i.IsListingImage,
                    IsSelected = false
                }).ToList();

            // create new contractviewmodel and pass along listing images


            List<SpecialFeaturesViewModel> feats =
                specialFeats.Select(feat =>
                new SpecialFeaturesViewModel
                {
                    Feature = feat,
                    IsSeleted = false
                }).ToList();

            ContractViewModel contract = new()
            {
                ImagesToSelect = imageList,
                SpecialFeatures = feats
            };
            contract.ImagesToSelect = imageList;

            if (agents.Count() > 0)
            {
                ViewData["AgentID"] = new SelectList(agents, "AgentID", "FullName");
            }
            else
            {
                ModelState.AddModelError("AgentID", "No verifed agents in the system.");
                ModelState.ClearValidationState(nameof(contract));
            }

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", id);
            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text");
            return View(contract);
        }

        // POST: Listings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContractViewModel contractModel)
        {
            Models.Contract contract = new();

            Listing listing = new()
            {
                StreetAddress = contractModel.StreetAddress,
                Municipality = contractModel.Municipality,
                CityLocation = contractModel.CityLocation,
                Province = contractModel.Province,
                SalesPrice = (decimal)contractModel.SalesPrice,
                PostalCode = contractModel.PostalCode,
                Footage = contractModel.Footage,
                NumOfBaths = contractModel.NumOfBaths,
                NumOfRooms = contractModel.NumOfRooms,
                NumOfStories = contractModel.NumOfStories,
                TypeOfHeating = contractModel.TypeOfHeating,
                Features = contractModel.Features,
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID,
                Images = new List<Image>()
            };

            for (int i = 0; i < contractModel.SpecialFeatures.Count; i++)
            {
                if (contractModel.SpecialFeatures[i].IsSeleted)
                {
                    listing.SpecialFeatures += $"{contractModel.SpecialFeatures[i].Feature}. ";
                }
                if (contractModel.SpecialFeatures[i].NumOfBays != null)
                {
                    listing.SpecialFeatures += $"{contractModel.SpecialFeatures[i].NumOfBays}";
                }

            }

            if (contractModel.Footage <= 0)
            {
                ModelState.AddModelError("Footage", "Footage must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfStories <= 0)
            {
                ModelState.AddModelError("NumOfStories", "Number of stories must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfBaths <= 0)
            {
                ModelState.AddModelError("NumOfBaths", "Number of bathrooms must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfRooms < 0)
            {
                ModelState.AddModelError("NumOfRooms", "Number of bedrooms must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }

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

            if (listing.ContractSigned)
            {
                if (contractModel.AgentID == null)
                {
                    ModelState.AddModelError("AgentID", $"Cannot make a contract without an agent selected");
                    ModelState.ClearValidationState(nameof(contractModel));
                }
                else
                {
                    contract = new()
                    {
                        StartDate = DateTime.Now,
                        AgentID = contractModel.AgentID,
                        ListingID = 0
                    };

                    listing.ListingStatus = "Available";
                    contract.EndDate = contract.StartDate.AddMonths(3);
                }

            }
            else
            {
                listing.ListingStatus = "Not Available";
            }

            if (ModelState.IsValid)
            {

                _context.Listings.Add(listing);
                await _context.SaveChangesAsync();
                //after listing is added, add images to listing


                //List<int> additions =
                //contractModel.ImagesToSelect.Where(i => i.IsSelected).Select(i => i.ImageID).ToList();

                //foreach (int addition in additions)
                //{
                //    listing.Images.Add(_context.Image.Find(addition));
                //}

                //_context.Update(listing);
                //await _context.SaveChangesAsync();

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

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "FullName", listing.CustomerID);
            ViewData["AgentID"] = new SelectList(_context.Agents.Where(a => a.IsVerified), "AgentID", "FullName", contract.AgentID);
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

            List<string>? specialFeatures = listing.SpecialFeatures?.Split('.').Select(feat => feat.Trim()).ToList();
            List<SpecialFeaturesViewModel> feats = new();

            if (specialFeatures != null)
            {
                feats =
                   specialFeats.Select(feat =>
                   new SpecialFeaturesViewModel
                   {
                       Feature = feat,
                       IsSeleted = specialFeatures.Contains(feat),
                       NumOfBays = specialFeatures.Where(f => f.Contains(feat)).FirstOrDefault()?.ToString()
                   }).ToList();


            }
            else
            {
                feats =
                    specialFeats.Select(feat =>
                    new SpecialFeaturesViewModel
                    {
                        Feature = feat,
                        IsSeleted = false
                    }).ToList();
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
                SpecialFeatures = feats,
                ListingStatus = listing.ListingStatus,
                ContractSigned = listing.ContractSigned,
                Images = listing.Images,
                SalesPrice = listing.SalesPrice
            };


            if (contract != null)
            {
                model.ContractID = contract.ContractID;
                model.StartDate = contract.StartDate;
                model.EndDate = contract.EndDate;
                model.AgentID = contract.AgentID;

                ViewBag.AgentName = contract.ListingAgent.FullName;
            }

            else 
            {
                ViewData["AgentID"] = new SelectList(_context.Agents.Where(a => a.IsVerified == true), "AgentID", "FullName");
            }


            ViewData["ProvinceOptions"] = new SelectList(provinceOptions, "Value", "Text", listing.Province);

            ViewData["CustomerID"] = _context.Customers.FirstOrDefault(c => c.CustomerID == listing.CustomerID).FullName;

            return View(model);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContractViewModel contractModel)
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
                SalesPrice = (decimal)contractModel.SalesPrice,
                NumOfBaths = contractModel.NumOfBaths,
                NumOfRooms = contractModel.NumOfRooms,
                NumOfStories = contractModel.NumOfStories,
                TypeOfHeating = contractModel.TypeOfHeating,
                Features = contractModel.Features,
                ContractSigned = contractModel.ContractSigned,
                CustomerID = contractModel.CustomerID,
                Images = contractModel.Images,
            };


            if (contractModel.Footage <= 0)
            {
                ModelState.AddModelError("Footage", "Footage must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfStories <= 0)
            {
                ModelState.AddModelError("NumOfStories", "Number of stories must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfBaths <= 0)
            {
                ModelState.AddModelError("NumOfBaths", "Number of bathrooms must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }
            if (contractModel.NumOfRooms < 0)
            {
                ModelState.AddModelError("NumOfRooms", "Number of bedrooms must be a value greater than 0");
                ModelState.ClearValidationState(nameof(contractModel));
            }

            for (int i = 0; i < contractModel.SpecialFeatures.Count; i++)
            {
                if (contractModel.SpecialFeatures[i].IsSeleted)
                {
                    listing.SpecialFeatures += $"{contractModel.SpecialFeatures[i].Feature}. ";
                }
                if (contractModel.SpecialFeatures[i].NumOfBays != null)
                {
                    listing.SpecialFeatures += $"{contractModel.SpecialFeatures[i].NumOfBays}";
                }

            }

            Models.Contract contract = new();

            if (id != listing.ListingID)
            {
                return NotFound();
            }


            if (!!listing.ContractSigned)
            {

                if (contractModel.AgentID == null)
                {
                    ModelState.AddModelError("AgentID", "Contract must have an agent.");
                    ModelState.ClearValidationState(nameof(contractModel));
                }
                else 
                {
                    if (contractModel.ContractID == null)
                    {

                        contract = new()
                        {
                            StartDate = DateTime.Now,
                            EndDate = (DateTime)contractModel.StartDate,
                            ListingID = listing.ListingID,
                            AgentID = contractModel.AgentID,
                        };

                        contract.EndDate = contract.EndDate.AddMonths(3);
                        listing.ListingStatus = "Available";
                    }
                    else
                    {
                        contract = new()
                        {
                            ContractID = contractModel.ContractID
                        };
                        listing.ListingStatus = "Available";
                    }
                }
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


                    if (contract.ListingID != null)               
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

            ViewData["CustomerID"] = _context.Customers.FirstOrDefault(c => c.CustomerID == listing.CustomerID).FullName;
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

            return View(listing);
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
