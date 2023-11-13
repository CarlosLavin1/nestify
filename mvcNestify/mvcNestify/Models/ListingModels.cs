using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace mvcNestify.Models
{

    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public string? FilePath { get; set; }
        public string? Name { get; set; }
        [NotMapped]
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
        [NotMapped]
        public IFormFile? PostedFile { get; set; }

        public int? ListingId { get; set; }
        public virtual Listing? Listing { get; set; }

        public int? AgentId { get; set; }
        public virtual Agent? Agent { get; set; }
        public bool IsVisible { get; set; }
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
        [Display(Name = "City Location")]
        public string? CityLocation { get; set; }

        [NotMapped]
        [Display(Name = "Address")]
        public string? Address
        {
            get 
            {
                return $"{StreetAddress}, {Municipality}, {Province}";
            }
        }

        [Required]
        public double Footage { get; set; }

        [Required]
        [Display(Name = "Number of Baths")]
        public double NumOfBaths { get; set; }

        [Required]
        [Display(Name = "Number of Bedrooms")]
        public double NumOfRooms { get; set; }

        [Required]
        [Display(Name = "Stories")]
        public int NumOfStories { get; set; }

        [Required]
        [Display(Name = "Heating")]
        public string? TypeOfHeating { get; set; }

        [Required]
        public string? Features { get; set; }

        [Display(Name = "Special Features")]
        public string? SpecialFeatures { get; set; }

        [Display(Name = "Listing Status")]
        public string? ListingStatus { get; set; }

        [Required]
        [Display(Name = "Signed Contract")]
        public Boolean ContractSigned { get; set; }

        [Required]
        [Display(Name = "Listing Customer")]
        public int? CustomerID { get; set; }
        public virtual Customer? Customer { get; set; }

        public virtual ICollection<Contract>? Contract { get; set; }

        public virtual ICollection<Showing>? Showing { get; set; }

        public virtual List<Image>? Images { get; set; }

        [NotMapped]
        public List<Image>? VisibleListingImages {  get; set; } 
    }

    public class Contract
    {
        [Key]
        public int? ContractID { get; set; }

        [Required]
        [Display(Name = "Contract Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "Contract End Date")]
        public DateTime EndDate { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }

        [Required]
        [ForeignKey("Agent")]
        [Display(Name = "Listing Agent")]
        public int? AgentID { get; set; }
        public virtual Agent? ListingAgent { get; set; }


        [Required]
        [ForeignKey("Listing")]
        [Display(Name = "Listing")]
        public int? ListingID { get; set; }
        public virtual Listing? Listing { get; set; }

    }

    public class Showing
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int? ShowingID { get; set;}

        [Required]
        [Display(Name ="Customer")]
        public int? CustomerID { get; set; }
        public virtual Customer? Customer { get; set; }

        [Required]
        [Display(Name = "Listing")]
        public int? ListingID { get; set; }
        public virtual Listing? Listing { get; set; }

        [Required]
        [Display(Name = "Agent")]
        public int? AgentID { get; set; }
        public virtual Agent? Agent { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name ="Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        [Display(Name ="End Time")]
        public DateTime EndTime { get; set; }

        [StringLength(255, ErrorMessage = "Comments character length cannot be longer than 255 characters.")]
        public string? Comments { get; set; }
    }
}



