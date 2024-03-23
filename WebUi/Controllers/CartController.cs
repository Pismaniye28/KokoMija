using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using KokoMija.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using WebUi.Identity;
using WebUi.Models;
using static WebUi.Models.CartModel;
using Entity;
using WebUi.EmailServices;
using WebUi.Extensions;

namespace WebUi.Controllers
{
    [Authorize]
    [AutoValidateAntiforgeryToken]
    [Route("cart")]
    public class CartController : Controller
    {
        private readonly IEmailSender _emailSender;
        private RoleManager<IdentityRole> _roleManager;
        private ICartService _cartService;
        private UserManager<User> _userManager;
        private IOrderService _orderService;
        private IPhotoService _photoService;

        public CartController(ICartService cartService, UserManager<User> userManager, IOrderService orderService,IEmailSender emailSender,
        RoleManager<IdentityRole> roleManager,IPhotoService photoService)
        {
            _cartService = cartService;
            _userManager = userManager;
            _orderService = orderService;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _photoService = photoService;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            return View(new CartModel()
            {
                CartId = cart.Id,
                CartItems = cart.CartItems.Select(i => new CartItemModel()
                {
                    CartItemId = i.Id,
                    ProductId = i.ProductId,
                    Name = i.Product.ProductName,
                    Price = i.Product.IsInDiscount
                        ? (double)(i.Product.Price - ((i.Product.DiscountRate ?? 0) / 100.0) * (double)i.Product.Price)
                        : (double)i.Product.Price,
                    Quantity = i.Quantity,
                    Size = i.Size,        
                    Color = i.Color,       
                    ImageUrl = i.ImageUrl
                }).ToList()
            });
        }
        
        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var userId = _userManager.GetUserId(User);
            var userdata = await _userManager.FindByIdAsync(userId);
            var cart = _cartService.GetCartByUserId(_userManager.GetUserId(User));
            var lineItems = cart.CartItems.Select(i => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "pln", // Set your currency here
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = i.Product.ProductName, // Assuming 'Product' has a 'Name' property
                        Images = new List<string> { "http://localhost:5175/img/" + i.ImageUrl} // Assuming 'CartItemModel' has an 'ImageUrl' property
                    },
                    UnitAmountDecimal = (decimal)(i.Product.Price - ((i.Product.DiscountRate ?? 0) / 100.0) * (double)i.Product.Price) * 100, // Stripe expects the amount in cents
                },
                AdjustableQuantity = new SessionLineItemAdjustableQuantityOptions
                {
                    Enabled = true,
                    Minimum = 1,
                    Maximum = 10,
                },
                Quantity = i.Quantity,
            }).ToList();
           
           var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                {
                    "card",
                },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"http://localhost:5175/ThanksForThePurchase", // Set your success URL here
                CancelUrl = "http://localhost:5175", // Set your cancel URL here
                ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                {
                    AllowedCountries = new List<string>
                    {
                        "PL", 
                    },
                },
                
            };
            
        
            var service = new SessionService();
            Session session = await service.CreateAsync(options);
            var session_id = session.Id;
            TempData["session_id"] = session_id;
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        [HttpGet("/ThanksForThePurchase")]
        public async Task<IActionResult> Thankyou()
        {
            var service = new SessionService();
            var session_id = TempData["session_id"] as string;
            Session session = await service.GetAsync(session_id);

            if (session.PaymentStatus == "paid")
            {
                var userId = _userManager.GetUserId(User);
                var userdata = await _userManager.FindByIdAsync(userId);
                var cart = _cartService.GetCartByUserId(userId);

                // Assuming you have a method to get the shipping address and user information
                var shippingAddress = session.ShippingDetails;
                
                // Create an Order entity and populate it with information
                var order = new Order
                {
                    UserId = userId,
                    FirstName = session.CustomerDetails.Name,
                    LastName = userdata.LastName,
                    City = shippingAddress.Address.City,
                    ConversationId = session_id,
                    PaymentType = EnumPaymentType.CreditCard,
                    Address = shippingAddress.Address.Line1 + shippingAddress.Address.Line2,
                    Phone = shippingAddress.Phone,
                    OrderDate = DateTime.Now,
                    OrderNumber = session.Id,
                    PaymentId = session.PaymentIntentId,
                    OrderState = EnumOrderState.waiting,
                    Email = session.CustomerDetails.Email,
                    TotalPrice = (int)session.AmountTotal,
                    OrderItems = cart.CartItems.Select(cartItem => new OrderItem
                    {
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = (long)cartItem.Product.Price,
                        Quatation = (int)cartItem.Product.Quatation,
                        
                        // You may need to set other properties like Price based on your logic
                    }).ToList(),

                    // Set other properties like OrderDate, PaymentType, etc.
                };

                // Save the order to your database (using your business or data layer)
                _orderService.Create(order);

                // Clear the cart
                await _cartService.ClearCartAsync(cart.Id);
                // Satın alan kişiye mesaj gönderme
                TempData.Put("message",new AlertMessage(){
                Title="Thank you for your shopping",
                Message="Your product will be packed and delivered to the distribution company as soon as possible",
                AlertType="success",
                icon="fa-regular",
                icon2="fa-face-grin-wink fa-flip"
                });
                return Redirect("/");

            }
            else
            {
                return RedirectToAction("Index");
            }
        }


  

      [HttpPost("addtocart")]
        public async Task<IActionResult> AddToCart(int productId, int quantity, int imageId, string colorName, string sizeName)
        {
            var userId = _userManager.GetUserId(User);
            
            // Here you would need to fetch the image, color, and size based on the provided IDs/names
            // You can do this by querying your database using an ORM like Entity Framework Core
            
            // For demonstration purposes, let's assume you have a service method to fetch the image, color, and size
            var image = await _photoService.GetImageByIdAsync(imageId);
            var color = await _photoService.GetColorByNameAsync(colorName);
            var size = await _photoService.GetSizeByNameAsync(sizeName);

            if (image == null)
            {
                // Handle case where image is not found
                return NotFound();
            }

            if (color == null)
            {
                // Set a basic color name if color is not found
                colorName = "Basic";
            }

            if (size == null)
            {
                // Set a basic size name if size is not found
                sizeName = "Basic";
            }


            // Now you can use the fetched image, color, and size to add to the cart
            await _cartService.AddToCartAsync(userId, productId, quantity, colorName, size, image.ImageUrl);

            return RedirectToAction("Index");
        }

        [HttpPost("deletefromcart")]
        public async Task<IActionResult> DeleteFromCart(int productId)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.DeleteFromCartAsync(userId, productId);

            return RedirectToAction("Index");
        }


        private IActionResult GetOrders()
        {
            var userId = _userManager.GetUserId(User);
            var orders = _orderService.GetOrders(userId);

            var orderListModel = new List<OrderListModel>();
            OrderListModel orderModel;
            foreach (var order in orders)
            {
                orderModel = new OrderListModel();

                orderModel.OrderId = order.Id;
                orderModel.OrderNumber = order.OrderNumber;
                orderModel.OrderDate = order.OrderDate;
                orderModel.Phone = order.Phone;
                orderModel.FirstName = order.FirstName;
                orderModel.LastName = order.LastName;
                orderModel.Email = order.Email;
                orderModel.Address = order.Address;
                orderModel.City = order.City;
                orderModel.OrderState = order.OrderState;
                orderModel.PaymentType = order.PaymentType;

                orderModel.OrderItems = order.OrderItems.Select(i => new OrderItemModel()
                {
                    OrderItemId = i.Id,
                    Name = i.Product.ProductName,
                    Price = (double)i.Price,
                    Quantity = i.Quantity,
                }).ToList();

                orderListModel.Add(orderModel);
            }

            return View("Orders", orderListModel);
        }



    }
}
