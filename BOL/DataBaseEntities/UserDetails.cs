using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BOL.DataBaseEntities
{
    public class UserDetails
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.com$", ErrorMessage = "Email-Exception")]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Gender { get; set; } = string.Empty;
        [Required]
        public bool isActive { get; set; }
        [Required]
        public DateTime CreatedId { get; set; }
    }
}
