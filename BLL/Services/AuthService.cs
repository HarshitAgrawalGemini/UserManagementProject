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
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore;
























namespace BLL.Services
{
    internal class AuthService
    {

        private readonly DBConetext _context;
        private readonly IConfiguration configuration;
        AuthService(DBConetext context , IConfiguration config)
        {
            _context = context;
            configuration = config;
        }
        
        public async   Task<string> RegisterAsync(RegisterDTO dto)
        {
            var user = new UserDetails()
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DOB= dto.DOB,
                Email = dto.Email,
                Gender = dto.Gender,
                isActive =false,
                CreatedId= DateTime.Now
            };
            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            var PasswordHash = new PasswordHasher<UserDetails>().HashPassword(user, dto.Password)
            var pwdDetails = new PasswordDetails
            {
                            UserId = user.Id,
                            PasswordHash = PasswordHash
            };

            await _context.Passwords.AddAsync(pwdDetails);
            await _context.SaveChangesAsync();
            return "registered";
        }



        public async Task<string> LoginAsync(LoginDTO dto) {


            var user = await this._context.Users.FirstOrDefaultAsync (u=> u.Email== dto.Email);
            if (user == null || !user.isActive)
            {
                return "Invalid Email or Inactive User";
            }



            var passwordEntry = await this._context.Passwords.FirstOrDefaultAsync(p=> p.UserId == user.Id);
            if (passwordEntry==null)
            {
                return "Password not set";
            }
            var result = new PasswordHasher<UserDetails>().VerifyHashedPassword(user, passwordEntry.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                return "Invalid Password";
            }
            var cLaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ),
                new Claim(ClaimTypes.Name, user.FirstName  )
            };

            return "";
        }
    }
}
