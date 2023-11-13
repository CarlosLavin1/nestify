using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace mvcNestify.Models
{
    public class ContractViewModel
    {
        public int? ContractID { get; set; }

        [Display(Name = "Contract Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Contract End Date")]
        public DateTime? EndDate { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Sales Price")]
        public decimal? SalesPrice { get; set; }
        
        [Required]
        [Display(Name = "Listing Agent")]
        public int? AgentID { get; set; }

        [Display(Name = "Listing")]
        public int? ListingID { get; set; }

        [Display(Name = "Listing Customer")]
        public int? CustomerID { get; set; }

        [Required(ErrorMessage = "A valid street address is required")]
        [Display(Name = "Street Address")]
        [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$", ErrorMessage = "Please enter a valid address")]
        [StringLength(90, ErrorMessage = "Street address cannot be longer than 90 characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "A municipality name is required")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Please enter a valid municipality name")]
        [StringLength(90, ErrorMessage = "Municipality name cannot be longer than 90 characters")]
        public string? Municipality { get; set; }

        [Required]
        [Display(Name = "City Location")]
        public string? CityLocation { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$",
            ErrorMessage = "Please enter a valid postal code")]
        [StringLength(6, ErrorMessage = "Postal code cannot be longer than 6 characters")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

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
        public string TypeOfHeating { get; set; }

        [Required]
        public string Features { get; set; }

        [Display(Name = "Special Features")]
        public string? SpecialFeatures { get; set; }

        [Display(Name = "Listing Status")]
        public string? ListingStatus { get; set; }

        [Required]
        [Display(Name = "Signed Contract")]
        public Boolean ContractSigned { get; set; }
        [Display(Name = "Listing Images")]
        public List<Image>? Images { get; set; }
        public List<int>? ImagesToShow { get; set; }

    }
    public class ListingViewModel
    {
        [Display(Name = "Listing ID")]
        public int? ListingID { get; set; }

        public string? StreetAddress { get; set; }

        public string? Municipality { get; set; }

        public string? Province { get; set; }


        public string? Address
        {
            get
            {
                return $"{StreetAddress}, {Municipality}, {Province}";
            }
        }

        [Display(Name = "Agent ID")]
        public int? AgentID { get; set; }

        public int? CustomerID { get; set; }

        [Display(Name = "First Name")]
        public string? CustFirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string? CustMiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string? CustLastName { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerFullName
        {
            get
            {
                return (CustMiddleName != null && CustMiddleName.Length > 0) ?
                    $"{CustFirstName} {CustMiddleName} {CustLastName}" :
                    $"{CustFirstName} {CustLastName}";
            }
        }

        [Display(Name ="Contract Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Contract End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Signed Contract")]
        public Boolean ContractSigned { get; set; }

        [Display(Name = "Listing Status")]
        public string? ListingStatus { get; set; }

    }

    public class AgentListingViewModel
    {
        [DataType(DataType.Currency)]
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }

        [Display(Name = "Listing ID")]
        public int? ListingID { get; set; }

        public string? StreetAddress { get; set; }

        public string? Municipality { get; set; }
        
        [Display(Name = "City Location")]
        public string? CityLocation { get; set; }

        public string? Province { get; set; }

        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }


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
        public string TypeOfHeating { get; set; }

        [Required]
        public string Features { get; set; }

        [Display(Name = "Special Features")]
        public string SpecialFeatures { get; set; }

        [Display(Name = "Signed Contract")]
        public Boolean ContractSigned { get; set; }

        [Display(Name = "Contract Start Date")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "Contract End Date")]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Listing Status")]
        public string? ListingStatus { get; set; }

        [Display(Name = "Agent ID")]
        public int? AgentID { get; set; }

        [Display(Name = "First Name")]
        public string? AgentFirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string? AgentMiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string? AgentLastName { get; set; }

        [Display(Name = "Agent Name")]
        public string? AgentFullName
        {
            get
            {
                return (AgentMiddleName != null && AgentMiddleName.Length > 0) ?
                    $"{AgentFirstName} {AgentMiddleName} {AgentLastName}" :
                    $"{AgentFirstName} {AgentLastName}";
            }
        }

        [Display(Name = "Office Email")]
        public string? OfficeEmail { get; set; }

        [Display(Name = "Office Phone")]
        public string? OfficePhone { get; set; }

        public int? CustomerID { get; set; }

        [Display(Name = "First Name")]
        public string? CustFirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string? CustMiddleName { get; set; }

        [Display(Name = "Last Name")]
        public string? CustLastName { get; set; }

        [Display(Name = "Customer Name")]
        public string? CustomerFullName
        {
            get
            {
                return (CustMiddleName != null && CustMiddleName.Length > 0) ?
                    $"{CustFirstName} {CustMiddleName} {CustLastName}" :
                    $"{CustFirstName} {CustLastName}";
            }
        }
    }
}
