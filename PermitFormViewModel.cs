using System.ComponentModel.DataAnnotations;

namespace PermitApplication.Models
{
    public class PermitFormViewModel
    {
        [Required]
        public string Name { get; set; } = "";

        [Required]
        public string Street { get; set; } = "";

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string State { get; set; } = "FL"; // Default value

        [Required]
        public string Zip { get; set; } = "";

        [Required]
        public string County { get; set; } = "";

        [Required]
        public string PermitType { get; set; } = "Water Use Permit"; // Default value
    }
}
