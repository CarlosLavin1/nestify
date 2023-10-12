using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcNestify.Models
{
    public class Agent
    {
        [Key]
        public int AgentID { get; set; }

        [Required(ErrorMessage = "Agent's S.I.N number is required")]
        public string? AgentSIN { get; set; }

        [Required(ErrorMessage = "Agent's first name is required")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Agent's last name is required")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Agent's date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 7 characters (123-123-1234)")]
        [Display(Name = "Home Phone")]
        public string? HomePhone { get; set; }

        
        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 7 characters (123-123-1234)")]
        [Display(Name = "Cell Phone")]
        public string? CellPhone { get; set; }

        [Required(ErrorMessage = "Please provide an office phone number")]
        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 7 characters (123-123-1234)")]
        [Display(Name = "Office Phone")]
        public string? OfficePhone { get; set; }

        [Required(ErrorMessage = "An agent email is required")]
        [Display(Name = "Office Email")]
        public string? OfficeEmail { get; set; }

        [Required(ErrorMessage = "A valid street address is required")]
        [Display(Name = "Street Address")]
        [StringLength(90, ErrorMessage = "Street address cannot be longer than 90 characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "A municipality name is required")]
        [StringLength(90, ErrorMessage = "Municipality name cannot be longer than 90 characters")]
        public string? Municipality { get; set; }

        [Required(ErrorMessage = "Province is required")]
        [StringLength(3, ErrorMessage = "Please provide province abbreviation")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(6,ErrorMessage ="Postal code cannot be longer than 6 characters")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Agent username is required")]
        public string? Username { get; set; }

        public string AuthorizationLevel { get; set; } = "Agent";

        public int CreatorID { get; set; } = 0;
        public DateTime DateOfEmployment { get; set; } = DateTime.Now;

    }

    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }

        [Required(ErrorMessage = "Customer's first name is required")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Customer's last name is required")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "A valid street address is required")]
        [Display(Name = "Street Address")]
        [StringLength(90, ErrorMessage = "Street address cannot be longer than 90 characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "A municipality name is required")]
        [StringLength(90, ErrorMessage = "Municipality name cannot be longer than 90 characters")]
        public string? Municipality { get; set; }

        [StringLength(12, ErrorMessage = "Please enter a 10 digit phone number (123-123-1234)")]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        // use case asks for proof of identity
    }

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
        public DateTime? UploadTime { get; set; }
        public bool Validated { get; set; }
        public int StaffID { get; set; }
    }
}
