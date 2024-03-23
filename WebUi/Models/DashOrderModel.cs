using System.Collections.Generic;
using WebUi.Models; // Import the namespace where your OrderListModel is defined
using KokoMija.Entity;
using WebUi.Identity; // Import the namespace where your User and Order entities are defined

namespace WebUi.Models
{
    public class DashOrderModel
    {
        public List<Order> AllOrders { get; set; }
        public List<User> RecentUsers { get; set; }
        
    }
}