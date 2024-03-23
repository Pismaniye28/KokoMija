using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KokoMija.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stripe;
using Stripe.Checkout;

namespace WebUi.Models
{
   
        public class CartModel : PageModel
    {
        public int CartId { get; set; }
        public List<CartItemModel> CartItems { get; set; }

        public double TotalPrice()
        {
            return CartItems.Sum(i=>i.Price*i.Quantity);
        }
    }

        public class CartItemModel{
        public int CartItemId { get; set; }
        public int ProductId { get; set; }
        public string ImageUrl { get; set; }
        public string Color { get; set; }
        public string Size{get; set;}
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }

        }
    }
