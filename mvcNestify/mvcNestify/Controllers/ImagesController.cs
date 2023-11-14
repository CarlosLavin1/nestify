﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Create([Bind("ListingId, AgentId, FileName, Description, AltText, PostedFile, IsListingImage, IsAgentImage")] Image image)
        {
            IFormFile imageFile = image.PostedFile;
            int fileSizeLimit = 7000000;
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length <= fileSizeLimit)
                {
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
                    // populate model props
                    image.Name = fileName;
                    image.FilePath = fullPath;
                    image.UploadDateTime = DateTime.Now;
                    image.Validated = false;
                    image.StaffID = 20; // get staff id from authenticated staff
                    image.IsVisible = false;

                    _context.Add(image);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = $"Successfully uploaded file: " + fileName;
                    return View("Confirmation");
                }
                
                ViewBag.Message = "Error uploading file";
                return View("Confirmation");

            }
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
        public async Task<IActionResult> Edit(int id, [Bind("ImageID,FilePath,Name,Description,AltText,UploadDateTime,Validated,StaffID,ListingId,AgentId")] Image image)
        {
            int? temp = image.ListingId;
            if (id != image.ImageID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
