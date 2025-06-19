using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services
{
    public  class EmailService


    {

        private readonly IConfiguration configuration;

        public EmailService(IConfiguration config) { 
        this.configuration = config;
        }
        public async Task<bool> SendEmail(string Email ,string subject , string body)
        {
            try
            {

                var smtpClient = new SmtpClient("smtp.gmail.com")

                {
                    Credentials= new NetworkCredential(this.configuration["Email:Sender"], this.configuration["Email:Password"]),
                    EnableSsl= true,
                    Port = 587

                };

                var mailMessage = new MailMessage()
                {
                    From=new MailAddress(this.configuration["Email:Sender"]!),
                    Subject=subject,
                    Body=body,
                    IsBodyHtml=true
                };
                mailMessage.To.Add(Email);
                await smtpClient.SendMailAsync(mailMessage);
                return true;

                
            }
            catch (Exception ex){ 

                Console.WriteLine(ex.Message);
            }
            return false;

        }





    }
}
