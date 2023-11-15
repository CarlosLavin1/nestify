using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvcNestify.Models
{
    public class ImageSelectionViewModel
    {
        public int ImageID { get; set; }
        public string? FilePath { get; set; }
        public string? Name { get; set; }
        [Display(Name = "File Name")]
        [RegularExpression(@"^[^.]*$", ErrorMessage = "Please do not add a file extension to file name")]
        public string? FileName { get; set; }
        [Required(ErrorMessage = "Image description is required")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Alternate text is required")]
        [Display(Name = "Alternate Text")]
        public string? AltText { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public bool Validated { get; set; }
        public int StaffID { get; set; }
        //[NotMapped]
        //public IFormFile? PostedFile { get; set; }

        public int? ListingId { get; set; }
        public virtual Listing? Listing { get; set; }

        public int? AgentId { get; set; }
        public virtual Agent? Agent { get; set; }
        public bool IsVisible { get; set; }
        public bool IsListingImage { get; set; }
        public bool IsAgentImage { get; set; }
        public bool IsSelected { get; set; }
    }
}
