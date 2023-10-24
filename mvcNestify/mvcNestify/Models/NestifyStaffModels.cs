using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcNestify.Models
{
    public class Agent
    {
        [Key]
        public int AgentID { get; set; }

        [Required(ErrorMessage = "Agent's S.I.N number is required")]
        [RegularExpression(@"^\d{3}(-?)\d{3}\1\d{3}$", ErrorMessage = "Please enter a valid S.I.N")]
        [Display(Name = "Agent S.I.N Number")]
        public string? AgentSIN { get; set; }

        [Required(ErrorMessage = "Agent's first name is required")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }


        [Display(Name = "Middle Name")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Agent's last name is required")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }


        [NotMapped]
        [Display(Name = "Agent Name")]
        public string? FullName
        {
            get
            {
                return (MiddleName != null && MiddleName.Length > 0) ?
                    $"{FirstName} {MiddleName} {LastName}" :
                    $"{FirstName} {LastName}";
            }
        }

        [Required(ErrorMessage = "Agent's date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 12 characters")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Home Phone")]
        public string? HomePhone { get; set; }


        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 12 characters")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter a valid phone number")]
        [Display(Name = "Cell Phone")]
        public string? CellPhone { get; set; }

        [Required(ErrorMessage = "Please provide an office phone number")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter a valid phone number")]
        [StringLength(12, ErrorMessage = "Phone number cannot be longer than 12 characters")]
        [Display(Name = "Office Phone")]
        public string? OfficePhone { get; set; }

        [Required(ErrorMessage = "An agent email is required")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email")]
        [Display(Name = "Office Email")]
        public string? OfficeEmail { get; set; }

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

        [Required(ErrorMessage = "Agent username is required")]
        public string? Username { get; set; }

        [Required]
        public string AuthorizationLevel { get; set; } = "Agent";

        [Required]
        public int CreatorID { get; set; } = 0;

        [Required]
        public bool IsVerified { get; set; } = false;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Employment")]
        public DateTime DateOfEmployment { get; set; }

        public virtual ICollection<Listing>? Listing { get; set; }
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
        [RegularExpression(@"^[A-Za-z0-9]+(?:\s[A-Za-z0-9'_-]+)+$", ErrorMessage = "Please enter a valid address")]
        [StringLength(90, ErrorMessage = "Street address cannot be longer than 90 characters")]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "Province is required")]
        public string? Province { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]$", ErrorMessage = "Please enter a valid postal code")]
        [StringLength(6, ErrorMessage = "Postal code cannot be longer than 6 characters")]
        [Display(Name = "Postal Code")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "A municipality name is required")]
        [StringLength(90, ErrorMessage = "Municipality name cannot be longer than 90 characters")]
        public string? Municipality { get; set; }

        [StringLength(12, ErrorMessage = "Please enter a 10 digit phone number (123-123-1234)")]
        [Display(Name = "Phone Number")]
        [Required(ErrorMessage = "Phone number is required")]
        [RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter a valid phone number")]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public bool IsVerified { get; set; } = false;
        // use case asks for proof of identity

        public virtual ICollection<Listing>? Listing { get; set; }
    }

}