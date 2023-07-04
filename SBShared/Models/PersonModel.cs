using System.ComponentModel.DataAnnotations;

namespace SBShared.Models
{
    // standard 2.0 or less for backwards compatibility with .net framework 
    public class PersonModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
    }
}
