using Microsoft.AspNetCore.Mvc;

namespace mvcNestify.Controllers
{
    public class ImageUploadController : Controller
    {
        private IWebHostEnvironment Environment { get; set; }

        public ImageUploadController(IWebHostEnvironment environment)
        {
            Environment = environment;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        public IActionResult Index(IFormFile? postedFile)
        {
            const int fileSizeLimit = 1048576;

            if (postedFile != null && postedFile.Length <= fileSizeLimit)
            {
                string wwwpath = this.Environment.WebRootPath;
                string contentPath = this.Environment.ContentRootPath;

                string path = Path.Combine(wwwpath, "Uploads/TempFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return RedirectToAction("Index", "Home");

            }
            else
            {
                if (postedFile == null)
                    ViewBag.Message = $"File cannot be null";
                else
                    ViewBag.Message = $"File exceeded limitation of {fileSizeLimit}";
            }

            return View("Index");
        }
    }
}
