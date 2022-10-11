using System.ComponentModel.DataAnnotations;

namespace SunriseAuto.Models
{
    public class RegistrationModel
    {
        [Required]
        [EmailAddress]
        [Display(Name ="User Name")]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        public bool AcceptUserAgreement { get; set; }
        public string? RegistrationInValid { get; set; }
    }
}
