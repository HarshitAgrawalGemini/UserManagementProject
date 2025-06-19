using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BOL.DataBaseEntities
{
    public class EmailVerificationToken
    {
        public int Id { get; set; }
        public Guid Token { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime Expiry { get; set; }
    }
}
