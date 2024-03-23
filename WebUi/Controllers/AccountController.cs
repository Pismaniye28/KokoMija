using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebUi.EmailServices;
using WebUi.Extensions;
using WebUi.Identity;
using WebUi.Models;
using System.Security.Claims;
using Stripe;

namespace WebUi.Controllers
{   
    [AutoValidateAntiforgeryToken]
    [Route("account/")]
    public class AccountController:Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        private readonly ICartService _cartService;

        private readonly IEmailSender _emailSender;

        private readonly IOrderService _orderService;
        private readonly IFavoriteService _favoriteService;
        private readonly IRatingService _ratingService;
        private readonly IProductService _productService;
        private readonly ICommentService _commentService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        
        public AccountController(UserManager<User> userManager,
        SignInManager<User> signInManager,
        IEmailSender emailSender,
        ICartService cartService,
        IOrderService orderService,
        IProductService productService,
        IFavoriteService favoriteService,
        ICommentService commentService,
        IRatingService ratingService,
        IWebHostEnvironment webHostEnvironment
        )
        {
            _userManager = userManager;
            _signInManager =signInManager;
            _emailSender = emailSender;
            _cartService=cartService;
            _orderService = orderService;
            _productService = productService;
            _webHostEnvironment = webHostEnvironment;
            _commentService = commentService;
            _favoriteService = favoriteService;
            _ratingService = ratingService;
        }

 

        [HttpPost]
        public async Task DeleteFavorite(string userId, int productId){
            await _favoriteService.DeleteFavoriteAsync(userId,productId);
        }

        [AllowAnonymous]
        [HttpGet("externallogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl = "http://localhost:5175")
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        [AllowAnonymous]
        [HttpGet("ExternalLoginCallback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "http://localhost:5175", string remoteError = null)
        {
            // Handle the callback from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
            {
                // Handle the error or redirect to the registration page
                return RedirectToAction(nameof(Register));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                // User is successfully signed in
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, prompt the user to create an account
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = info.Principal.FindFirstValue(ClaimTypes.Email) });
            }
        }

 
        
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("Account/AddFavorite")]
    public async Task<IActionResult> AddFavorite(int productId)
        {
            var userId = _userManager.GetUserId(User);

            var favorite = new FavoriteProduct
            {
                UserId = userId,
                ProductId = productId
            };

            if (await _favoriteService.IsFavoriteAsync(userId, productId))
            {
                await _favoriteService.DeleteFavoriteAsync(userId, productId);
            }
            else
            {
                await _favoriteService.AddFavoriteAsync(favorite);
            }

            // Determine the referring URL
            var referringUrl = Request.Headers["Referer"].ToString();

            // If the referring URL is empty or null, redirect to a default page
            if (string.IsNullOrEmpty(referringUrl))
            {
                return RedirectToAction("Index", "Home"); // Change to your default page
            }

            return Redirect(referringUrl);
        }

[Authorize]
[HttpGet("favori")]
public async Task<IActionResult> Favori()
{
    var userId = _userManager.GetUserId(User);

    if (userId == null)
    {
        return RedirectToAction("Login");
    }

    var favProducts = await _favoriteService.GetFavoriteProductsSortedByDateAsync(userId);
    var ratedProducts = await _ratingService.GetProductsRatedByUserAsync(userId);

    // Fetch average rating and rating count for rated products
    foreach (var product in ratedProducts)
    {
        product.AverageRating = await _ratingService.GetAverageRatingForProductAsync(product.ProductId);
        product.RatingCount = await _ratingService.GetRatingCount(product.ProductId);
        product.IsFavorite = await _favoriteService.IsFavoriteAsync(userId, product.ProductId);

        // Handle the case when AverageRating is null
        if (product.AverageRating == null)
        {
            // Set a default value or handle it accordingly
            product.AverageRating = 0.0;
        }
    }

    var viewModel = new UserDashboardViewModel
    {
        FavoriteProducts = favProducts,
        RatedProducts = ratedProducts
    };

    return View(viewModel);
}



  
        [Authorize]
        [HttpGet("profile")]
        public async Task <IActionResult> Manage()
        { 
            
                // Get the alert message from TempData
                if (TempData.ContainsKey("alertMessage"))
                {
                    ViewBag.AlertMessage = TempData["alertMessage"] as AlertMessage;
                }

            var userId = _userManager.GetUserId(User);
            var userdata = await _userManager.FindByIdAsync(userId);
            var orders =_orderService.GetOrders(userId);

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

                orderModel.OrderItems = order.OrderItems.Select(i=>new OrderItemModel(){
                        OrderItemId = i.Id,
                        Name = i.Product.ProductName,
                        Price = (double)i.Price,
                        Quantity = i.Quantity,
                       
                        
                       
                }).ToList();

                orderListModel.Add(orderModel);
            }
            
            var combinedViewModel = new UserProfileModel
            {
                UserId = userId,
                UserName = userdata.UserName,
                FirstName = userdata.FirstName,
                LastName = userdata.LastName,
                Email = userdata.Email,
                profilePic = userdata.ProfileImg,
                orderModel = orderListModel
            };

            
           
            return View(combinedViewModel);
        }

        

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("Account/UpdateProfile")]
        public async Task<IActionResult> UpdateProfile(IFormFile file)
        {
            var userId = _userManager.GetUserId(User);
            var userdata = await _userManager.FindByIdAsync(userId);

            if (file != null && file.Length > 0) // Check if a file is uploaded
            {
                var extension = Path.GetExtension(file.FileName);
                var profileImg = $"{Guid.NewGuid()}{extension}";

                var filePath = Path.Combine("img", profileImg);
                var serverFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath);

                using (var stream = new FileStream(serverFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                userdata.ProfileImg = profileImg;
                await _userManager.UpdateAsync(userdata);
            }

            TempData.Put("message",new AlertMessage(){
            Title="Zdjęcie profilowe",
            Message=$"Twoje zdjęcie profilowe zostało pomyślnie usunięte",
            AlertType="success",
            icon="fa-solid",
            icon2="fa-vial-circle-check fa-beat-fade"
            });

            return RedirectToAction("Manage");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/DeleteOlProfilePicture")]
        public async Task<IActionResult> DeleteOlProfilePicture(){
            var userId = _userManager.GetUserId(User);
            var userdata = await _userManager.FindByIdAsync(userId);         
            if (!string.IsNullOrEmpty(userdata.ProfileImg))
            {
                var protectedFilenames = new List<string> { "avatar(m).jpg", "avatar(w).jpg", "profile.png" };
                var profilePicPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", userdata.ProfileImg);
                if (protectedFilenames.Contains(userdata.ProfileImg))
                {
                    userdata.ProfileImg = null;
                    await _userManager.UpdateAsync(userdata);
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Zdjęcie profilowe",
                        Message = $"Twoje zdjęcie profilowe '{userdata.ProfileImg}' zostało pomyślnie usunięte",
                        AlertType = "warning",
                        icon = "fa-solid",
                        icon2 = "fa-vial-circle-check fa-beat-fade"
                    });
                }else{
                    // Check if the file exists
                    if (System.IO.File.Exists(profilePicPath))
                    {
                        // Delete the file
                        System.IO.File.Delete(profilePicPath);
                        Console.WriteLine($"File '{profilePicPath}' deleted successfully.");
                        // Update the ProfileImg property to null
                        userdata.ProfileImg = null;
                        // Save the changes to the database
                        await _userManager.UpdateAsync(userdata);
                        TempData.Put("message",new AlertMessage(){
                        Title="Zdjęcie profilowe",
                        Message=$"Twoje zdjęcie profilowe zostało pomyślnie usunięte",
                        AlertType="success",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });
                        return RedirectToAction("Manage");     
                        }else{        
                        TempData.Put("message",new AlertMessage(){
                        Title="Pojawił się problem",
                        Message=$"{userdata.ProfileImg} nie mogliśmy znaleźć zdjęcia o tej nazwie, prosimy o przesłanie do naszego zespołu ds. rozwoju",
                        AlertType="danger",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });
                        Console.WriteLine($"File '{profilePicPath}' does not exist.");
                        return RedirectToAction("Manage");
                    }
            }}

        // Add the appropriate return statement in case the profile image is "profile.png" or not found.
        // For example, you can return a view or redirect to another action.
        return RedirectToAction("Manage");

    }


        [HttpGet("/login")]
        public IActionResult Login(string ReturnUrl=null){
            return View(new LoginModel(){
                ReturnUrl = ReturnUrl
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/login")]  
        public async Task<IActionResult> Login(LoginModel model){

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                ModelState.AddModelError("","Şifre ve ya Email hatalı");
                return View(model);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError("","Lütfen Email hesabınıza gelen link ile hesabınızı onaylayın");
                return View(model);
            }
            var result = await _signInManager.PasswordSignInAsync(user,model.Password,false,false);
            if (result.Succeeded)
            {
                _cartService.InitializeCart(user.Id);
                return Redirect(model.ReturnUrl??"~/");
            }

            ModelState.AddModelError("","Şifre ve ya Email hatalı");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/register")]
        public async Task<IActionResult> Register(RegisterModel model){
            if (!ModelState.IsValid)
            {
                return View(model);
            }
                var user = new User(){
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    
                };
            
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {   
                await _userManager.AddToRoleAsync(user,"customer");
                //token oluşturma ve mail gönderme
                //token oluşturma
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var url = Url.Action("ConfirmEmail","Account",new{
                    userId = user.Id,
                    token = code
                });
                //Email
                await _emailSender.SendEmailAsync(model.Email,
                //Mail başlığı
                "Potwierdź swoje konto.",
                $"Lütfen email hesabınızı onaylamak için linke <a href='https://localhost:7070{url}'>tıklayınız.</a>"
                );


                return RedirectToAction("Login","Account");
            }
            ModelState.AddModelError("RePassword","Şifreniz aynı olmalıdır!");
            ModelState.AddModelError("","Bilinmeyen bir hata oluştu");
            return View(model);
        }
        [HttpGet("/register")]
        public IActionResult Register(){
            return View();
        }
        [Route("logout")]
        public async Task<IActionResult>Logout(){

        TempData.Put("message",new AlertMessage(){
        Title="Wylogowany",
        Message="Do zobaczenia wkrótce ^^",
        AlertType="warning",
        icon="fa-regular",
        icon2="fa-face-grin-wink fa-flip"
        });
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }
        
        [Route("Account/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                TempData.Put("message", new AlertMessage
                {
                    Title = "Nieprawidłowy Token",
                    Message = "Token zabezpieczający witrynę wygasł lub nie został przetworzony!",
                    AlertType = "danger",
                    icon = "fa-solid",
                    icon2 = "fa-user-ninja fa-beat-fade"
                });
                return View();
            }

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData.Put("message", new AlertMessage
                {
                    Title = "Nie znaleziono użytkownika",
                    Message = "Żądany użytkownik nie został znaleziony, można skontaktować się z właścicielem witryny lub utworzyć nowego użytkownika.",
                    AlertType = "danger",
                    icon = "fa-solid",
                    icon2 = "fa-person-circle-question fa-beat-fade"
                });
                return View();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                // Initialize the user's cart
                _cartService.InitializeCart(user.Id);

                // Create a Stripe customer account for the user
                var options = new CustomerCreateOptions
                {
                    Email = user.Email,
                    PaymentMethod = "pm_card_visa", // Replace with the desired payment method
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = "pm_card_visa", // Replace with the desired default payment method
                    },
                };

                var service = new CustomerService();
                service.Create(options);

                TempData.Put("message", new AlertMessage
                {
                    Title = "Twoje konto zostało zatwierdzone",
                    Message = "Można z powodzeniem rozpocząć zakupy :)",
                    AlertType = "success",
                    icon = "fa-regular",
                    icon2 = "fa-thumbs-up fa-beat-fade"
                });

                return View();
            }

            TempData.Put("message", new AlertMessage
            {
                Title = "Twoje konto nie zostało zatwierdzone!",
                Message = "Nie mogliśmy potwierdzić Twojego konta! Spróbuj ponownie.",
                AlertType = "warning",
                icon = "fa-solid",
                icon2 = "fa-skull-crossbones fa-beat-fade"
            });

            return View();
        }

        [HttpGet("/forgotpassword")]
        public IActionResult ForgotPassword(){
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/forgotpassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPassword model){
            if (string.IsNullOrEmpty(model.Email))
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                return View(model);
            }
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword","Account",new{
                    userId = user.Id,
                    token = code
                });
             await _emailSender.SendEmailAsync(model.Email,
                //Mail başlığı
                "Hesabınızın Şifresini Değiştirin.",
                $"Lütfen email hesabınızın şifresini linke tıklayarak değiştirin <a href='https://localhost:7070{url}'>tıklayınız.</a>"
                );
            return View(model);
        }
        [HttpGet("/resetpassword")]
        public IActionResult ResetPassword(string userId,string token){
            if (userId ==null || token == null)
            {
                     TempData.Put("message",new AlertMessage(){
                    Title="Zmiana hasła nie została potwierdzona",
                    Message="Ta zmiana hasła nie została autoryzowana ze względów bezpieczeństwa Kod:1",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
                return RedirectToAction("Home","Index");
            }
            var model = new ResetPasswordModel {Token=token};

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/resetpassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model){
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user==null)
            {
                   TempData.Put("message",new AlertMessage(){
                    Title="Zmiana hasła nie została potwierdzona",
                    Message="Ta zmiana hasła nie została autoryzowana ze względów bezpieczeństwa Kod:2",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
                return RedirectToAction("Home","Index");
            }
            var result = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Login", "Account");
            }
              TempData.Put("message",new AlertMessage(){
                    Title="Wystąpił błąd",
                    Message="Wystąpił błąd Error:404",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-user-ninja fa-beat-fade"
                });
            return View(model);
        }

        public IActionResult AccessDenied(){
            return View();
        }
    }
}