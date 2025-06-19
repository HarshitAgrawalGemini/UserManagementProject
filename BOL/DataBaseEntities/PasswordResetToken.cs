using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL.DataBaseEntities
{
    public class PasswordResetToken
    {
        [Key]
        public Guid Token { get; set; }

        [Required]
        public string Email { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }
    }
}
