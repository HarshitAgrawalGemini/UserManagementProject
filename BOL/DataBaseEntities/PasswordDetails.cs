using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BOL.DataBaseEntities
{
    public class PasswordDetails
    {
        [Key]
        public  int Id { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public int  UserId { get; set; }

        [ForeignKey("UserId")]
        public UserDetails User { get; set; }
    }
}
