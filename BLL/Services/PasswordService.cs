using BOL.DataBaseEntities;
using BOL.DTOs;
using DAL.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class PasswordService
    {
        private readonly DBConetext _context;
        private readonly IConfiguration configuration;
        private readonly EmailService _emailService;
        public PasswordService(DBConetext context, IConfiguration config) {
            _context = context;
            configuration = config;
            _emailService = new EmailService(configuration);
        }








        public async Task<string> SendResetEmailAsync(string email)
        {
            //EmailService _emailService = new EmailService(this.configuration);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return "User not found";

            var token = Guid.NewGuid();
            var expiry = DateTime.UtcNow.AddMinutes(15);

            var resetToken = new PasswordResetToken
            {
                Token = token,
                Email = email,
                Expiry = expiry
            };

            _context.Tokens.Add(resetToken);
            await _context.SaveChangesAsync();

            var resetLink = $"http://127.0.0.1:5020/api/user/reset-password?email={email}&token={token}";
            await _emailService.SendEmail(email, "Reset Your Password", $"Click to reset your password: {resetLink}");

            return "Password reset email sent";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var parsedToken = Guid.TryParse(dto.Token, out var tokenGuid);
            if (!parsedToken)
                return "Invalid token format";

            var tokenEntry = await _context.Tokens
                .FirstOrDefaultAsync(t => t.Token == tokenGuid && t.Email == dto.Email);

            if (tokenEntry == null || tokenEntry.Expiry < DateTime.UtcNow)
                return "Token is invalid or expired";

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return "User not found";

            Console.WriteLine("NEW PASSWORD => " + dto.NewPassword);

            var PasswordHash = new PasswordHasher<UserDetails>().HashPassword(user, dto.NewPassword);
            var pwdDetails = new PasswordDetails
            {
                UserId = user.Id,
                PasswordHash = PasswordHash
            };


            var ExistingPWD = await this._context.Passwords.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (ExistingPWD == null)
            {
                this._context.Add(pwdDetails);
            }
            else
            {
                ExistingPWD.PasswordHash = PasswordHash;
                this._context.Passwords.Update(ExistingPWD);
            }
            await this._context.SaveChangesAsync();

            //user. = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword); // use your preferred hashing method
            _context.Tokens.Remove(tokenEntry); // cleanup token
            await _context.SaveChangesAsync();

            return "Password successfully reset";
        }

    }
}
