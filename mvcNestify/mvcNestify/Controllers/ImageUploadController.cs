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
        public IActionResult Index()
        {
            return View("Index");
        }

        public IActionResult Index(IFormFile? postedFile)
        {
            const int fileSizeLimit = 1048576;

            if (postedFile != null && postedFile.Length <= fileSizeLimit)
            {
                string fileType = postedFile.ContentType;

                string wwwpath = this.Environment.WebRootPath;
                string contentPath = this.Environment.ContentRootPath;

                switch (fileType)
                {
                    case "image/png":
                        ViewBag.Type = $"This is a .png filetype";
                        break;
                    case "image/webp":
                        ViewBag.Type = $"This is a .webp compressed file";
                        break;
                    default:
                        ViewBag.Type = $"Unsupported file type: {fileType}";
                        break;
                }

                string path = Path.Combine(wwwpath, "Uploads/TempFiles");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = Path.GetFileName(postedFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                    ViewBag.Message = $"FileUploaded {fileName}";
                }

            }
            else
            {
                if (postedFile == null)
                    ViewBag.Message = $"File cannot be null";
                else
                    ViewBag.Message = $"File exceeded limitation of {fileSizeLimit}";
            }

            return Redirect("Home/Index");
        }
    }
}
