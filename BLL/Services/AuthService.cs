using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Context;
using BOL.DTOs;
using BOL.DataBaseEntities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            return "";
        }
    }
}
