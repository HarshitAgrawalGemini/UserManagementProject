using BOL.DataBaseEntities;
using BOL.DTOs;
using DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService
    {
        private readonly DBConetext _context;
        private readonly IConfiguration configuration;
        private readonly EmailService _emailService;


        public UserService(DBConetext context, IConfiguration config) {

            _context = context;
            configuration = config;
            _emailService = new EmailService(configuration);
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

        public async Task<UserDetails> GetProfileById(int id)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == id);
            return user;
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
            var user = this._context.Users.FirstOrDefault(u => u.Id == id);
            if (user != null)
            {
                user.FirstName = Updates.FirstName;
                user.LastName = Updates.LastName;
                user.Gender = Updates.Gender;
                user.Email = Updates.Email;
                user.DOB = Updates.DOB;

                await this._context.SaveChangesAsync();
                return "done";
            }
            return null;
        }


        public async Task<string?> DeleteUser(int id)
        {


            try
            {

                UserDetails? user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {

                    this._context.Users.Remove(user);
                    await this._context.SaveChangesAsync();
                    return "done";
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return null;
        }


    }
}
