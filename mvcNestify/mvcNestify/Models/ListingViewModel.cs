using System.ComponentModel.DataAnnotations;

namespace mvcNestify.Models
{
    public class ListingViewModel
    {
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

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
