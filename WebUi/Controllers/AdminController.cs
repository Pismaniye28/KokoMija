using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Entity;
using KokoMija.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WebUi.Extensions;
using WebUi.Identity;
using WebUi.Models;
using static WebUi.Models.RoleModel;
using Stripe;

namespace WebUi.Controllers
{
    [Authorize(Roles ="Admin")]
    [Route("admin/")]
    public class AdminController: Controller
    {
        private readonly ApplicationContext _context;
        private IProductService _productService;
        private ICategoryService _categoryService;
        private ICourserService _courserService;
        private RoleManager<IdentityRole> _roleManager;
        private UserManager<User> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        private IPhotoService _photoService;

        private IOrderService _orderService;

        public AdminController(IProductService productService,
        ICategoryService categoryService ,
        ICourserService courserService,
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager,
        IPhotoService photoService,
        IOrderService orderService,
        IWebHostEnvironment webHostEnvironment,
        ApplicationContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _courserService = courserService;
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
            _photoService = photoService;
            _orderService =orderService;
            _webHostEnvironment = webHostEnvironment;
        }
    [HttpGet("orders")]
    public async Task<IActionResult> Orders()
    {
        var allTheOrders = await _orderService.GetAllOrders();
        // Assuming there is a service to get recent users
        var dashOrderModel = new DashOrderModel
        {
            AllOrders = allTheOrders,
        };

        return View(dashOrderModel);
    }
[HttpPost]
[ValidateAntiForgeryToken]
[Route("orders/edit")]
public async Task<IActionResult> OrdersEdit()
{
    return View();
}

[HttpGet("dashboard")]
public async Task<IActionResult> Dashboard()
{
    // Retrieve recent orders and users, and calculate total orders and users
    var completedOrders = await _orderService.GetCompletedOrders();
    var completedOrderModels = completedOrders.Select(order => new OrderListModel
    {
        OrderNumber = order.OrderNumber,
        OrderDate = order.OrderDate,
        Address = order.Address,
        OrderState = order.OrderState,
        City = order.City,
        Email = order.Email,
        PaymentType = order.PaymentType,
        FirstName = order.FirstName,
        LastName = order.LastName,
        Phone = order.Phone,
        Note = order.Note,
        UserId = order.UserId,
        OrderId = order.Id,
        OrderItems = order.OrderItems.Select(item => new OrderItemModel
        {
            OrderItemId = item.Id,
            Price = (long)item.Price,
            Name = item.Product.ProductName,
            Quantity = item.Quantity,
            // Map other properties as needed
        }).ToList(),
        // Map other properties as needed
    }).ToList();
    
    // You should have a GetUserService or similar
    var totalCompletedOrders = completedOrders.Count;

    // Create a DashboardModel instance and populate it with the retrieved data
    var dashboardModel = new DashboardModel
    {
        CompletedOrders = completedOrderModels,
        TotalOrders = totalCompletedOrders,
        // Map other properties as needed
    };

    // Pass the populated DashboardModel to the view
    return View(dashboardModel);
}



        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("user/delete")]
        public async Task<IActionResult> UserDelete(string userId)
        {
            // Retrieve the user
            var user = await _userManager.FindByIdAsync(userId);   
            if (!string.IsNullOrEmpty(user.ProfileImg))
            {
                var protectedFilenames = new List<string> { "avatar(m).jpg", "avatar(w).jpg", "profile.png" };
                var profilePicPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", user.ProfileImg);
                if (protectedFilenames.Contains(user.ProfileImg))
                {
                    user.ProfileImg = null;
                    await _userManager.UpdateAsync(user);
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "Zdjęcie profilowe",
                        Message = $"Twoje zdjęcie profilowe '{user.ProfileImg}' zostało pomyślnie usunięte",
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
                        user.ProfileImg = null;
                        // Save the changes to the database
                        await _userManager.UpdateAsync(user);
                        TempData.Put("message",new AlertMessage(){
                        Title="Zdjęcie profilowe",
                        Message=$"Twoje zdjęcie profilowe zostało pomyślnie usunięte",
                        AlertType="success",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });   
                        }else{        
                        TempData.Put("message",new AlertMessage(){
                        Title="Pojawił się problem",
                        Message=$"{user.ProfileImg} nie mogliśmy znaleźć zdjęcia o tej nazwie, prosimy o przesłanie do naszego zespołu ds. rozwoju",
                        AlertType="danger",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });
                        Console.WriteLine($"File '{profilePicPath}' does not exist.");
                    }
                 }
            }

            if (user == null)
            {
                // User not found
                return NotFound();
            }

            // Check if the user is associated with a Stripe customer
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                // User is associated with a Stripe customer, attempt to delete it
                try
                {
                    var customerService = new CustomerService();
                    var deleteOptions = new CustomerDeleteOptions { };
                    await customerService.DeleteAsync(user.StripeCustomerId, deleteOptions);

                    // Stripe customer deleted successfully, now delete the user profile
                    var result = await _userManager.DeleteAsync(user);

                    if (result.Succeeded)
                    {
                        // User profile deleted successfully
                        return RedirectToAction("UserList"); // Redirect to the user list page
                    }
                    else
                    {
                        // Handle user profile deletion errors
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                        return View("UserDelete", userId); // Return to the delete confirmation page with errors
                    }
                }
                catch (StripeException stripeEx)
                {
                    // Handle Stripe customer deletion errors
                    ModelState.AddModelError("", $"Error deleting Stripe customer: {stripeEx.Message}");
                    return View("UserDelete", userId); // Return to the delete confirmation page with errors
                }
            }
            else
            {
                // No Stripe customer associated with the user, delete the user profile directly
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    // User profile deleted successfully
                    return RedirectToAction("UserList"); // Redirect to the user list page
                }
                else
                {
                    // Handle user profile deletion errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View("UserDelete", userId); // Return to the delete confirmation page with errors
                }
            }
        }

        [HttpGet("user/new")]
        public IActionResult UserCreate(UserCreateModel model)
        {

            // Get all available roles
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
            
            // Assuming you have a UserCreateModel instance, set the available roles in the model
            model.AllRoles = allRoles;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("user/new")]
        public async Task<IActionResult> UserCreate(UserCreateModel model, string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    EmailConfirmed = model.EmailConfirmed,
                    ProfileImg = model.SelectedProfilePicture
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add user to selected roles
                    selectedRoles = selectedRoles ?? new string[] { };
                    await _userManager.AddToRolesAsync(user, selectedRoles);

                    if (model.IsStripeCustomer)
                    {
                        // Create a Stripe customer here
                        var customerService = new CustomerService();
                        var customerCreateOptions = new CustomerCreateOptions
                        {
                            Email = user.Email,
                            PaymentMethod = "pm_card_visa",
                            InvoiceSettings = new CustomerInvoiceSettingsOptions
                            {
                                DefaultPaymentMethod = "pm_card_visa",
                            },
                        };

                        var stripeCustomer = await customerService.CreateAsync(customerCreateOptions);

                        // Save the Stripe customer ID to the user
                        user.StripeCustomerId = stripeCustomer.Id;
                        await _userManager.UpdateAsync(user); // Update the user to save the Stripe customer ID
                    }

                    return RedirectToAction("UserList"); // Redirect to the user list page after successful creation
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description); // Add any identity errors to ModelState
                    }
                }
            }

            // If ModelState is not valid or user creation fails, return to the create user page with error messages
            model.AllRoles = _roleManager.Roles.Select(r => r.Name).ToList(); // Refresh the roles list
            return View(model);
        }

    
        [HttpGet("user/edit/{id}")]
        public async Task<IActionResult> UserEdit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user!=null)
            {
                var selectedRoles = await _userManager.GetRolesAsync(user);
                var roles = _roleManager.Roles.Select(i=>i.Name);

                ViewBag.Roles = roles;
                return View(new UserDetailsModel(){
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    SelectedRoles = selectedRoles
                });
            }
            return RedirectToAction("UserList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("user/edit/{id}")]
        public async Task<IActionResult> UserEdit(UserDetailsModel model,string[] selectedRoles)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if(user!=null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.EmailConfirmed = model.EmailConfirmed;
                    var options = new CustomerCreateOptions
                    {
                        Email = model.Email,
                        PaymentMethod = "pm_card_visa",
                        InvoiceSettings = new CustomerInvoiceSettingsOptions
                        {
                            DefaultPaymentMethod = "pm_card_visa",
                        },
                    };

                    var result = await _userManager.UpdateAsync(user);

                    if(result.Succeeded)
                    {
                        var userRoles = await _userManager.GetRolesAsync(user);
                        selectedRoles = selectedRoles?? new string[]{};
                        await _userManager.AddToRolesAsync(user,selectedRoles.Except(userRoles).ToArray<string>());
                        await _userManager.RemoveFromRolesAsync(user,userRoles.Except(selectedRoles).ToArray<string>());

                        return RedirectToAction("UserList");
                    }
                }
                return Redirect("UserList");
            }

            return View(model);

        }
        [HttpGet("user/list")]
        public IActionResult UserList()
        {   
            return View(_userManager.Users);
        }
        [HttpGet("user/role/edit/{id?}")]
        public async Task<IActionResult> RoleEdit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            var members = new List<User>();
            var nonmembers = new List<User>();
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                var isInRole = await _userManager.IsInRoleAsync(user, role.Name);
                var list = isInRole ? members : nonmembers;
                list.Add(user);
            }
            var model = new RoleDetails()
            {
                Role = role,
                Members = members,
                NonMembers = nonmembers
            };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("user/role/edit/{id?}")]  
        public async Task<IActionResult> RoleEdit(RoleEditModel model)
        {
            if(ModelState.IsValid)
            {
                foreach (var userId in model.IdsToAdd ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.AddToRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
          
                foreach (var userId in model.IdsToDelete ?? new string[]{})
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if(user!=null)
                    {
                        var result = await _userManager.RemoveFromRoleAsync(user,model.RoleName);
                        if(!result.Succeeded)
                        {
                              foreach (var error in result.Errors)
                              { 
                                ModelState.AddModelError("", error.Description);  
                              }  
                        }
                    }
                }
            }
            return Redirect("/admin/role/"+model.RoleId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/RoleDelete")]
        public async Task<IActionResult> RoleDelete(string roleName)
        {
            if (!string.IsNullOrEmpty(roleName))
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var result = await _roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        // Role deleted successfully
                        return RedirectToAction("RoleList", "Admin"); // Redirect to an appropriate page
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                else
                {
                    // Role with the provided name does not exist
                    ModelState.AddModelError("", "Role not found.");
                }
            }
            else
            {
                // Invalid or empty role name
                ModelState.AddModelError("", "Invalid role name.");
            }

            // Handle errors or redirect as needed
            return View("Error"); // You can redirect to an error page or handle errors as needed
        }

        [HttpGet("user/role/list")]
        public IActionResult RoleList()
        {
            return View(_roleManager.Roles);
        }
        [HttpGet("user/role/new")]
        public IActionResult RoleCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("user/role/new")]      
        public async Task<IActionResult> RoleCreate(RoleModel model)
        {
            if(ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(model.name));
                if(result.Succeeded)
                {
                    return RedirectToAction("RoleList");
                }else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                }
            }
            return View(model);
        }
        [HttpGet("product/list")]
        public async Task<IActionResult> ProductList()
        {
            var products = await _productService.GetAll();
            return View(new ProductListViewModel()
            {
                Products = products

            });
        }
        [HttpGet("category/list")]
        public async Task< IActionResult> CategoryList()
        {
            var categories = await _categoryService.GetAll();
            return View(new CategoryListViewModel()
            {
                Categories = categories
            });
        }

        [HttpGet("product/new")]
        public IActionResult ProductCreate()
        {
            return View(new ProductModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/new")]
        public  IActionResult ProductCreate(ProductModel model)
        {


            if(ModelState.IsValid)
            {

                var entity = new KokoMija.Entity.Product()
                {
                    ProductName = model.ProductName,
                    Url = model.Url,
                    Price = model.Price,
                    Description = model.Description,
                    
                };
                if(_productService.Create(entity))
                {
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.ProductName} isimli category eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 
                    return RedirectToAction("ProductList");
                }
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{_productService.ErrorMessage} isimli ürün eklenemedi!.",
                    AlertType="danger",
                    icon="fa-solid",
                    icon2="fa-thumbs-down fa-beat-fade"
                    });       
                return View(model);
            }        
            return View(model);         
        }

        [HttpGet("category/new")]
        public IActionResult CategoryCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/category/create")]
        public IActionResult CategoryCreate(CategoryModel model)
        {
             if(ModelState.IsValid)
            {
                var entity = new Category()
                {
                    Name = model.Name,
                    Url = model.Url            
                };
                
                _categoryService.Create(entity);
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.Name} isimli category eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 


                return RedirectToAction("CategoryList");
            }
            return View(model);
        }


        [HttpGet("product/edit/{id:int}")]
        public  async Task<IActionResult> ProductEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _productService.GetByIdWithCategories((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new ProductModel()
                {
                    ProductId = entity.ProductId,
                    ProductName = entity.ProductName,
                    Url = entity.Url,
                    Price = entity.Price,
                    Description = entity.Description,
                    IsInDiscount = entity.IsInDiscount,
                    DiscountRate = entity.IsInDiscount ? entity.DiscountRate : null,
                    Quatations = entity.Quatation,
                    IsApproved = entity.IsApproved,
                    IsHome = entity.IsHome,
                    AllCategories = await _categoryService.GetAll(),
                    AllImages = await _photoService.GetAll(),
                    SelectedImages = entity.ProductImages.Select(pi => pi.Image).ToList(),
                    SelectedCategories = entity.ProductCategories.Select(pc => pc.Category).ToList()
                };
                
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("product/edit/{id:int}")]
        public async Task<IActionResult> ProductEdit(ProductModel model, int[] categoryIds, int[] selectedImages)
        {
            if (ModelState.IsValid)
            {
                var entity = await _productService.GetById(model.ProductId);
                if (entity == null)
                {
                    return NotFound();
                }
                // Update the entity properties from the model
               
                entity.IsInDiscount = model.IsInDiscount;
                entity.DiscountRate = model.IsInDiscount ? model.DiscountRate : null;
                entity.IsHome = model.IsHome;
                entity.Price = model.Price;
                entity.Description = entity.Description;
                entity.ProductName = model.ProductName;
                entity.Url = model.Url;
                entity.IsApproved = model.IsApproved;

                if (_productService.Update(entity, categoryIds, selectedImages))
                {
                    TempData.Put("message", new AlertMessage()
                    {
                        Title = "kayıt güncellendi",
                        Message = $"{entity.ProductName} isimli ürün güncellendi.",
                        AlertType = "success",
                        icon = "fa-solid",
                        icon2 = "fa-vial-circle-check fa-beat-fade"
                    });
                    return RedirectToAction("ProductList");
                }

                TempData.Put("message", new AlertMessage()
                {
                    Title = "kayıt güncellenemedi",
                    Message = $"{_productService.ErrorMessage} isimli hata oluştu.",
                    AlertType = "danger",
                    icon = "fa-solid",
                    icon2 = "fa-thumbs-down fa-beat-fade"
                });
            }

            // Repopulate the model properties here

            return View(model);
        }

       
        [HttpGet("category/edit/{id:int}")]
        public IActionResult CategoryEdit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var entity = _categoryService.GetByIdWithProducts((int)id);

            if(entity==null)
            {
                 return NotFound();
            }

            var model = new CategoryModel()
            {
                CategoryId = entity.CategoryId,
                Name = entity.Name,
                Url = entity.Url,
                Products = entity.ProductCategories.Select(p=>p.Product).ToList()
            };
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("category/edit/{id:int}")]
        public async Task<IActionResult> CategoryEdit(CategoryModel model)
        {
            if(ModelState.IsValid)
            {
                var entity = await _categoryService.GetById(model.CategoryId);
                if(entity==null)
                {
                    return NotFound();
                }
                entity.Name = model.Name;
                entity.Url = model.Url;

                _categoryService.Update(entity);

                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.Name} isimli category güncellendi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                return RedirectToAction("CategoryList");
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/DeleteProduct")] 
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var entity = await _productService.GetById(productId);

            if(entity!=null)
            {
                await _productService.DeleteAsync(entity);
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.ProductName} isimli ürün silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });

            return RedirectToAction("ProductList");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/deletecategory")]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var entity = await _categoryService.GetById(categoryId);

            if(entity!=null)
            {
                _categoryService.Delete(entity);
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.Name} isimli category silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
            return RedirectToAction("CategoryList");
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("admin/deletefromcategory")]
        public IActionResult DeleteFromCategory(int productId,int categoryId)
        {
            _categoryService.DeleteFromCategory(productId,categoryId);
            return Redirect("/admin/category/edit/"+categoryId);
        }
        [HttpGet("slider/list")]
        public async Task< IActionResult> CourserList(){
            var afis = await _courserService.GetAll();
            return View(new SliderModel(){
                slider = afis
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/DeleteCourser")]
        public async Task<IActionResult> DeleteCourser(int courserId){
            var entity = await _courserService.GetById(courserId);
            if (entity!=null)
            {
                var PicPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", entity.CourserImgUrl);
                  if (System.IO.File.Exists(PicPath))
                    {
                        // Delete the file
                        System.IO.File.Delete(PicPath);
                        _courserService.Delete(entity);
                        Console.WriteLine($"File '{PicPath}' deleted successfully.");
                        TempData.Put("message",new AlertMessage(){
                        Title="Zdjęcie profilowe",
                        Message=$"Twoje zdjęcie profilowe zostało pomyślnie usunięte",
                        AlertType="success",
                        icon="fa-solid",
                        icon2="fa-vial-circle-check fa-beat-fade"
                        });   
                    }
            }
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.CourserImgUrl} isimli afiş silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                return RedirectToAction("CourserList");
        }
        [HttpGet("slider/edit/{id:int}")]
        public async Task<IActionResult> EditCourser(int? id){
              if(id==null)
            {
                return NotFound();
            }
                var entity = await _courserService.GetById((int) id);
                if(entity==null)
            {
                 return NotFound();
            }

                return View(new Courser(){
                    CourserId = entity.CourserId,
                    CourserImgUrl = entity.CourserImgUrl,
                });
        }

        [Authorize]
        [HttpPost]
        [Route("slider/edit/{id:int}")]
        public async Task<IActionResult> EditCourser(int? id,IFormFile file)
            {
                var entity = await _courserService.GetById((int)id);
                        if (file != null)
                        {
                            var extantion = Path.GetExtension(file.FileName);
                            var dateImgForSlider = string.Format($"{Guid.NewGuid()}{extantion}");
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForSlider);
                            var deletepath = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",file.FileName);
                            entity.CourserImgUrl= dateImgForSlider;
                            

                            using (var stream = new FileStream(path,FileMode.Create))
                            {
                            await file.CopyToAsync(stream);
                            }
                        }
                        entity.CourserId = (int)id;
                        
                        _courserService.Update(entity);
                        return RedirectToAction("CourserList");

              }

        [HttpGet("slider/new")]
        public IActionResult CourserCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("slider/new")]
        public async Task<IActionResult> CourserCreate(Courser model,IFormFile file)
        {
                    if (file != null)
                        {
                            var extantion = Path.GetExtension(file.FileName);
                            var dateImgForSlider = string.Format($"{Guid.NewGuid()}{extantion}");
                            var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForSlider);
                            model.CourserImgUrl= dateImgForSlider;
                            

                            using (var stream = new FileStream(path,FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            
                        }
                        _courserService.Create(model);
                        
            return RedirectToAction("CourserList");
        }
        [HttpGet("image/list")]
        public async Task< IActionResult> ImageList(ImgListModel model)
        {
            var images= await _photoService.GetAll();
            return View(new ImgListModel(){
                Images = images
            });
        }
        [HttpGet("image/new")]
        public IActionResult ImageCreate(){
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("image/new")]
        public async Task<IActionResult> ImageCreate(ImgCreateModel model, IFormFile file){
            if (ModelState.IsValid)
            {   
                    var entity = new Image(){};
                            if (!model.ClearColors)
                                {
                                    entity.ColorName = model.colorName;
                                    entity.ColorCode = model.colorCode;
                                }else{
                                    entity.ColorName = null;
                                    entity.ColorCode = null;
                                }
 
                    if (file != null)
                    {
                        var extantion = Path.GetExtension(file.FileName);
                        var dateImgForProduct = string.Format($"{Guid.NewGuid()}{extantion}");
                        var path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot\\img",dateImgForProduct);   
                        entity.ImageUrl=dateImgForProduct;
                        using (var stream = new FileStream(path,FileMode.Create))
                        {
                        await file.CopyToAsync(stream);
                        }
                    }else{
                        return View(model);
                    }
                   
                     _photoService.Create(entity);
                    TempData.Put("message",new AlertMessage(){
                    Title="kayıt güncellendi",
                    Message=$"{entity.ImageUrl} isimli resim eklendi.",
                    AlertType="success",
                    icon="fa-solid",
                    icon2="fa-vial-circle-check fa-beat-fade"
                    }); 
                    return RedirectToAction("ImageList");
            }else{
                return View(model);
                } 
         }
        [HttpPost]
        [Route("Admin/ImageDelete")]
        public async Task<IActionResult> ImageDelete(int imageId){
            var entity = await _photoService.GetById(imageId);
            if (entity!=null)
            {
                var PicPath = Path.Combine(_webHostEnvironment.WebRootPath, "img", entity.ImageUrl);
                  if (System.IO.File.Exists(PicPath))
                    {
                        // Delete the file
                        System.IO.File.Delete(PicPath);

                    }

            }
                
                TempData.Put("message",new AlertMessage(){
                Title="Silme işlemi onaylandı",
                Message=$"{entity.ImageId} isimli Image silindi.",
                AlertType="success",
                icon="fa-solid",
                icon2="fa-vial-circle-check fa-beat-fade"
                });
                await _photoService.DeleteAsync(entity);
                return RedirectToAction("ImageList");
        }
        [HttpGet("image/edit/{id:int}")]
        public async Task<IActionResult> ImageEdit(int? id)
        {
             if(id==null)
            {
                return NotFound();
            }
                var entity = await _photoService.GetById((int) id);
                if(entity==null)
            {
                 return NotFound();
            }
                return View(new ImgEditModel(){
                    colorCode = entity.ColorCode,
                    colorName = entity.ColorName,
                    Medium = entity.SizeMedium,
                    xsmall = entity.SizeXSmall,
                    Small = entity.SizeSmall,
                    xlarge = entity.SizeXLarge,
                    Large = entity.SizeLarge,
                    imageId = entity.ImageId,
                    imageUrl = entity.ImageUrl
                });
        }
        [HttpPost]
        [Route("image/edit/{id:int}")]
        public async Task<IActionResult> ImageEdit(ImgEditModel model,int? id)
        {
                var entity = await _photoService.GetById((int)id);
                  if(entity==null)
                    {
                        return NotFound();
                    }
                    if (ModelState.IsValid)
                    {    
                        if (!model.ClearColors)
                        {
                            entity.ColorName = model.colorName;
                            entity.ColorCode = model.colorCode;
                        }else{
                            entity.ColorName = null;
                            entity.ColorCode = null;
                        }
                        entity.ImageUrl = entity.ImageUrl;
                        entity.ImageId = entity.ImageId;
                        entity.SizeMedium = model.Medium;
                        entity.SizeLarge = model.Large;
                        entity.SizeXLarge =model.xlarge;
                        entity.SizeSmall = model.Small;
                        entity.SizeXSmall = model.xsmall;
                    _photoService.Update(entity);
                    return RedirectToAction("ImageList");
                    }
                return View(model);
    }
}
}