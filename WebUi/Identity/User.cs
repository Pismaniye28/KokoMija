using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Entity;

namespace WebUi.Identity
{
    public class User:IdentityUser
    {
        public string FirstName{get;set;}
        public string LastName { get; set; }
        
        #nullable enable
        public string? ProfileImg { get; set; }
        public string? StripeCustomerId { get; set; }
    }
}