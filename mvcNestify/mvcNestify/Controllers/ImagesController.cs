using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mvcNestify.Data;
using mvcNestify.Models;

namespace mvcNestify.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment Environment { get; set; }

        public ImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            Environment = environment;
        }

        // GET: Images
        public async Task<IActionResult> Index()
        {
            List<Microsoft.AspNetCore.Identity.IdentityUser> users = _context.Users.ToList();
            ViewBag.Staff = users;
              return _context.Image != null ? 
                          View(await _context.Image.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Image'  is null.");
        }

        // GET: Images/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Image == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // GET: Images/Create
        public IActionResult Create()
        {
            List<SelectListItem> agents = new List<SelectListItem>();
            List<SelectListItem> listings = new List<SelectListItem>();
            foreach (var agent in _context.Agents)
                agents.Add(new SelectListItem { Text = $"{agent.FullName}", Value = $"{ agent.AgentID}" });
            foreach (var listing in _context.Listings)
                listings.Add(new SelectListItem { Text = $"{listing.Customer.FullName} - {listing.Address}", Value = $"{listing.ListingID}" });

            ViewData["Listings"] = listings;
            ViewData["Agents"] = agents;
            return View();
        }

        // POST: Images/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ListingId,AgentId,FileName,Description,AltText,PostedFile, IsListingImage, IsAgentImage, Type")] Image image)
        {
            IFormFile imageFile = image.PostedFile;
            int fileSizeLimit = 7000000;
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length <= fileSizeLimit)
                {
                    bool test = image.IsListingImage;
                    // verify valid image type
                    string contentType = imageFile.ContentType;

                    if (contentType != "image/jpeg" && 
                        contentType != "image/png" && 
                        contentType != "image/gif" && 
                        contentType != "image/svg" && 
                        contentType != "image/webp")
                    {
                        ModelState.AddModelError("PostedFile", "File type not supported");
                        return View(image);
                    }

                    string wwwpath = this.Environment.WebRootPath;
                    var filePath = Path.Combine(wwwpath, "Uploads/TempFiles");
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                    }

                    var fileExtension = Path.GetExtension(imageFile.FileName);
                    var fileName = image.FileName == null ? Path.GetFileName(imageFile.FileName) : image.FileName + fileExtension;
                    var fullPath = Path.Combine(filePath, fileName);

                    using (FileStream stream = new FileStream(fullPath, FileMode.Create))
                    {
                        imageFile.CopyTo(stream);
                    }
                    //check if listing already has 7 images
                    if(image.ListingId != null)
                    {
                        Listing? listing = _context.Listings?.Find(image.ListingId);
                        if(listing?.Images?.Count == 7)
                        {
                            ModelState.AddModelError("", "Listing already has 7 images");
                            return View(image);
                        }
                    }
                    // populate model props
                    image.Name = fileName;
                    image.FilePath = fullPath;
                    image.UploadDateTime = DateTime.Now;
                    image.Validated = false;
                    image.StaffID = User.FindFirstValue(ClaimTypes.NameIdentifier); // get staff id from authenticated staff
                    image.IsVisible = false;
                    if (image.Type == "L" && image.AgentId == null && image.ListingId == null)
                        image.IsListingImage = true;
                    else if (image.Type == "A" && image.AgentId == null && image.ListingId == null)
                        image.IsAgentImage = true;

                    _context.Add(image);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = $"Successfully uploaded file: " + fileName;
                    return View("Confirmation");
                }
                
                ViewBag.Message = "Error uploading file";
                return View("Confirmation");

            }
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            return View(image);
        }

        // GET: Images/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Image == null)
            {
                return NotFound();
            }

            var image = await _context.Image.FindAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return View(image);
        }

        // POST: Images/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,FilePath,Name,Description,AltText,UploadDateTime,Validated,StaffID,ListingId,AgentId,IsVisible,IsListingImage,IsAgentImage")] Image image)
        {
            int? temp = image.ListingId;
            if (id != image.ImageID)
            {
                return NotFound();
            }

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (image.StaffID == userId)
            {
                ModelState.AddModelError("Validated", "Creator of image cannot verify image.");
                ModelState.ClearValidationState(nameof(image));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<Microsoft.AspNetCore.Identity.IdentityUser> users = _context.Users.ToList();
                    if (image.Validated)
                        image.VerifiedBy = users.First(s => s.Id == User.FindFirstValue(ClaimTypes.NameIdentifier)).Email;
                    image.ListingId = temp;
                    _context.Update(image);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImageExists(image.ImageID))
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
            return View(image);
        }

        // GET: Images/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Image == null)
            {
                return NotFound();
            }

            var image = await _context.Image
                .FirstOrDefaultAsync(m => m.ImageID == id);
            if (image == null)
            {
                return NotFound();
            }

            return View(image);
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Image == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Image'  is null.");
            }
            var image = await _context.Image.FindAsync(id);
            if (image != null)
            {
                _context.Image.Remove(image);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
          return (_context.Image?.Any(e => e.ImageID == id)).GetValueOrDefault();
        }
    }
}
