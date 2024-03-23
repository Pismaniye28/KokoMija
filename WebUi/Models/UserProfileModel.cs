using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;

namespace WebUi.Models
{
    public class UserProfileModel 
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string profilePic { get; set; }

        public List<OrderListModel> orderModel{get;set;}
    }
}