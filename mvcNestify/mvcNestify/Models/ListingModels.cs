using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvcNestify.Models
{

    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public string? FilePath { get; set; }
        public string? Name { get; set; }
        [Required(ErrorMessage = "Image description is required")]
        public string? Description { get; set; }
        [Required(ErrorMessage = "Alternate text is required")]
        [Display(Name = "Alternate Text")]
        public string? AltText { get; set; }
        public DateTime? UploadDateTime { get; set; }
        public bool Validated { get; set; }
        public int StaffID { get; set; }
        [NotMapped]
        public IFormFile? PostedFile { get; set; }
    }

    public class Listing
    {
        public int ListingID { get; set; }

        [Required(ErrorMessage = "A valid street address is required")]
        [Display(Name = "Street Address")]
        [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$", ErrorMessage = "Please enter a valid address")]
        [StringLength(90, ErrorMessage = "Street address cannot be longer than 90 characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "A municipality name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter a valid municipality name")]
        [StringLength(90, ErrorMessage = "Municipality name cannot be longer than 90 characters")]
        public string? Municipality { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$",
            ErrorMessage = "Please enter a valid postal code")]
        [StringLength(6, ErrorMessage = "Postal code cannot be longer than 6 characters")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Required]
        public string? CityLocation { get; set; }

        [Required]
        public double Footage { get; set; }

        [Required]
        public double NumOfBaths { get; set; }

        [Required]
        public double NumOfRooms { get; set; }

        [Required]
        public int NumOfStories { get; set; }

        [Required]
        public string TypeOfHeating { get; set; }

        [Required]
        public string Features { get; set; }

        public string SpecialFeatures { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal SalesPrice { get; set; }

        public Boolean ContractSigned { get; set; }

        [Required]
        public int? AgentID { get; set; }
        public virtual Agent ListingAgent { get; set; }

        [Required]
        public int? CustomerID {get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string? ListingStatus { get; set; }

        public virtual ICollection<Showing>? Showing { get; set; }
    }

    public class Showing 
    {

        [Required]
        public int? CustomerID { get; set; }
        public virtual Customer Customer { get; set; }

        [Required]
        public int? ListingID {  get; set; }
        public virtual Listing Listing { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime {  get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndTime { get; set; }

        [StringLength(255, ErrorMessage = "Comments character length cannot be longer than 255 characters.")]
        public string? Comments { get; set; }
    }
}



