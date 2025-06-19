using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using BOL.DataBaseEntities;
using System.Threading.Tasks;

namespace DAL.Context
{
    public class DBConetext:DbContext
    {
        public DBConetext(DbContextOptions<DBConetext> options):base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: Customize relationships if needed
            modelBuilder.Entity<UserDetails>()
                .HasOne<PasswordDetails>()
                .WithOne(p => p.User)
                .HasForeignKey<PasswordDetails>(p => p.UserId);

            base.OnModelCreating(modelBuilder);
        }
        public DbSet<UserDetails> Users { get; set; }
        public DbSet<PasswordDetails> Passwords { get; set; }

        public DbSet<PasswordResetToken> Tokens { get; set; }
        public DbSet<EmailVerificationToken> RegisterVerificationTokens { get; set; }
    }
}
