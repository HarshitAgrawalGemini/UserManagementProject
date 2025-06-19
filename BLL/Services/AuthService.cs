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

                //return "Create Password email sent";

                //var PasswordHash = new PasswordHasher<UserDetails>().HashPassword(user, dto.Password);
                //var pwdDetails = new PasswordDetails
                //{
                //    UserId = user.Id,
                //    PasswordHash = PasswordHash
                //};

                //await _context.Passwords.AddAsync(pwdDetails);
                //await _context.SaveChangesAsync();
                return "registered";
            }
            catch (Exception ex) {

                return ex.ToString();
            }
            
            }



        public async Task<string?> LoginAsync(LoginDTO dto)
        {

            EmailService emailService = new EmailService(this.configuration);
            //bool emailSent = await emailService.SendEmail(dto.Email, "Reset Your Password", "this is the testing mail");
            //Console.WriteLine( "EMAILSENT   "+  emailSent);
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
                new Claim(ClaimTypes.Name, user.FirstName  )
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(

                issuer: this.configuration["Jwt:Issuer"],
                audience: this.configuration["Jwt:Audience"],
                claims: Claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(this.configuration["Jwt:DurationInMinutes"]!)),
                signingCredentials: creds

                );



            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<UpdateProfileDTO> GetProfileDTOAsync(int userid)
        {
            var user = await _context.Users.FindAsync(userid);
            if (user == null)
            {
                return null;
            }
            else
            {

                return new UpdateProfileDTO()
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DOB = user.DOB,
                    Gender = user.Gender,
                    Email = user.Email
                };
            }
        }

        public  async Task<UserDetails> GetProfileById(int id)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(u=>u.Id == id );
            return user ;
        }



        public async Task<List<UserDetails>> ListUsers()
        {
            var users = this._context.Users.ToList();
            foreach (var item in users)
            {
                Console.WriteLine(item);
            }
            return users; 
        }

        public async Task<string?> UpdateUserProfile(int id, UpdateProfileDTO Updates)
        {
            var user = this._context.Users.FirstOrDefault(u=>u.Id == id);
            if (user != null)
            {
                user .FirstName = Updates.FirstName;
                user .LastName = Updates.LastName;
                user .Gender = Updates.Gender;
                user .Email = Updates.Email;
                user.DOB = Updates.DOB;

                await this._context.SaveChangesAsync();
                return "done";
            }
            return null;
        }


        public  async Task<string?> DeleteUser(int id) {
        

            try {

                UserDetails? user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == id) ;
                if(user != null)
                {

                 this._context.Users.Remove(user);
                    await this._context.SaveChangesAsync();
                    return "done";
                }
            }
            catch(Exception ex) {
                return null;
            }
            return null;
        }


        //public async Task<string> GeneratePasswordResetTokenAsync(string email)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null) return null;

        //    var token = Guid.NewGuid().ToString();
        //    user.ResetToken = token;
        //    user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
        //    await _context.SaveChangesAsync();
        // j
        //    return token;
        //}

        //public async Task<string> SendPasswordResetEmail(string email)
        //{
        //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null) return "User not found";

        //    user.ResetToken = Guid.NewGuid();
        //    user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(15);
        //    await _context.SaveChangesAsync();

        //    string resetLink = $"https://yourfrontend.com/reset-password?email={email}&token={user.ResetToken}";

        //    await _emailService.SendEmailAsync(email, "Reset Password", $"Click here to reset your password: {resetLink}");

        //    return "Reset email sent";
        //}



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

            Console.WriteLine("NEW PASSWORD => "+  dto.NewPassword);

            var PasswordHash = new PasswordHasher<UserDetails>().HashPassword(user, dto.NewPassword);
            var pwdDetails = new PasswordDetails
            {
                UserId = user.Id,
                PasswordHash = PasswordHash
            };


            var ExistingPWD = await  this._context.Passwords.FirstOrDefaultAsync(p=>p.UserId==user.Id)  ;
            if(ExistingPWD == null)
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
