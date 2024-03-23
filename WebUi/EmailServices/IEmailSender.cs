using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebUi.EmailServices
{
    public interface IEmailSender
    {
        //smtp
        //api

        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}