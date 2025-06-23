using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using BOL.DTOs;
using BOL.DataBaseEntities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
//using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.WebSockets;



namespace BLL.Services
{
    public class AuthService
    {

        private readonly DBConetext _context;
        private readonly IConfiguration configuration;
        private readonly EmailService _emailService;
        public AuthService(DBConetext context, IConfiguration config)
        {
            _context = context;
            configuration = config;
            _emailService = new EmailService(configuration);
        }





















        public async Task<string> RegisterAsync(RegisterDTO dto)
        {
            try
            {
                var validity = await this._context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
                if (validity != null)
                {
                    return "User Exists";
                }
                var user = new UserDetails()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    DOB = dto.DOB,
                    Email = dto.Email,
                    Gender = dto.Gender,
                    isActive = false,
                    CreatedId = DateTime.Now
                };
                await this._context.Users.AddAsync(user);
                await this._context.SaveChangesAsync();


                var token = Guid.NewGuid();
                var expiry = DateTime.UtcNow.AddMinutes(15);

                var resetToken = new PasswordResetToken
                {
                    Token = token,
                    Email = dto.Email,
                    Expiry = expiry
                };

                _context.Tokens.Add(resetToken);
                await _context.SaveChangesAsync();

                var resetLink = $"http://127.0.0.1:5020/api/user/create-password?email={dto.Email}&token={token}";
                await _emailService.SendEmail(dto.Email, "Reset Your Password", $"Click to reset your password: {resetLink}");

               
                return "registered";
            }
            catch (Exception ex) {

                return ex.ToString();
            }
            
            }



        public async Task<string?> LoginAsync(LoginDTO dto)
        {

            
            var user = await this._context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                Console.WriteLine(user + "  <= this was user ");
                Console.WriteLine();
                Console.WriteLine( "Invalid Email or Inactive User");
                return null;
            }



            var passwordEntry = await this._context.Passwords.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (passwordEntry == null)
            {
                Console.WriteLine( "Password not set");
                return null;
            }
            var result = new PasswordHasher<UserDetails>().VerifyHashedPassword(user, passwordEntry.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                 Console.WriteLine("Invalid Password");
                return null;
            }
            var Claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ),
                //new Claim(ClaimTypes.Name, user.FirstName  ) here ClaimTypes.Name is a reserved keyword for claims in asp.net 
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(

                issuer: this.configuration["Jwt:Issuer"],
                audience: this.configuration["Jwt:Audience"],
                claims: Claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds

                );



            return new JwtSecurityTokenHandler().WriteToken(token);
        }






    }
}
