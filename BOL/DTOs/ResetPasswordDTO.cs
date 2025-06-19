using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace BOL.DTOs
{
    public class ResetPasswordDTO
    {
        [Required , EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
        [Required, Compare("NewPassword" , ErrorMessage="Password-Problem")]
        public string ConfirmPassword { get; set; } = string.Empty;
        [Required]
        public string Token {  get; set; } = string.Empty;
    }
}
