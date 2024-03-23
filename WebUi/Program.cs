using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using WebUi.Identity;
using WebUi.EmailServices;
using WebUi.Extensions;
using Data.Abstract;
using Data.Concrete.EfCore;
using Bussines.Abstract;
using Bussines.Concrete;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("MsSqlConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
var port = config.GetSection("EmailSender").GetValue<int>("Port");
var host = config.GetSection("EmailSender").GetValue<string>("Host");
var enablessl = config.GetSection("EmailSender").GetValue<bool>("EnableSSL");
var username = config.GetSection("EmailSender").GetValue<string>("UserName");
var password = config.GetSection("EmailSender").GetValue<string>("Password");
StripeConfiguration.ApiKey = config.GetSection("Secrets").GetValue<string>("Stripe");
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ShopContext>(options=>
    options.UseSqlServer(connectionString),ServiceLifetime.Scoped);
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(connectionString),ServiceLifetime.Scoped);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Use Google as the external auth provider
    }).AddCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        })
   .AddGoogle(options =>
  {
    IConfigurationSection googleAuthNSection =
    config.GetSection("Authentication:Google");
    options.ClientId = "558794277223-bunkfd7fm1sl1m3kf0r4g493pkar1tq8.apps.googleusercontent.com";
    options.ClientSecret = "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";
    options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
})
   .AddFacebook(options =>
  {
    IConfigurationSection FBAuthNSection =
    config.GetSection("Authentication:FB");
    options.ClientId = "317360560709702";
    options.ClientSecret = "40e0abe7d9079577002a035765693390";
    options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
});
   builder.Services.Configure<IdentityOptions>(options=>{
    //password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase=true;
    options.Password.RequireUppercase=true;
    options.Password.RequiredLength=8;
    options.Password.RequireNonAlphanumeric=false;
    //Lockout
    options.Lockout.MaxFailedAccessAttempts=4;
    options.Lockout.DefaultLockoutTimeSpan=TimeSpan.FromMinutes(4);
    options.Lockout.AllowedForNewUsers=true;
    //User
    options.User.RequireUniqueEmail=true;
    options.User.AllowedUserNameCharacters="aąbcćdeęfghijklłmnńoópqrsśtuvwxyzźżabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.SignIn.RequireConfirmedEmail=true;
    options.SignIn.RequireConfirmedPhoneNumber=false;
});
//cookie alanı
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/accessdenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.Cookie = new CookieBuilder {
        HttpOnly = true,
        Name = ".KokoMija.Security.Cookie",
        SameSite = SameSiteMode.None, // Set to None
        SecurePolicy = CookieSecurePolicy.Always // Set to Always
    };
});

    // Service
    builder.Services.AddScoped<IUnitOfWork,UnitOfWork>();
    builder.Services.AddScoped<IEmailSender,SmtpEmailSender>(i=> new SmtpEmailSender(host,port,enablessl,username,password));
    builder.Services.AddScoped<ICategoryService,CategoryManager>();
    builder.Services.AddScoped<IProductService,ProductManager>();
    builder.Services.AddScoped<ICourserService,CourserManager>();
    builder.Services.AddScoped<IPhotoService,PhotoManager>();
    builder.Services.AddScoped<ICartService,CartManager>();
    builder.Services.AddScoped<IOrderService,OrderManager>();
    builder.Services.AddScoped<IRatingService,RatingManager>();
    builder.Services.AddScoped<IFavoriteService,FavoriteManager>();
    builder.Services.AddScoped<ICommentService,CommentManager>();
    builder.Services.AddScoped<UserManager<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      var scopeFactory = app.Services.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var cartService = scope.ServiceProvider.GetRequiredService<ICartService>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            SeedIdentity.Seed(userManager, roleManager,cartService,configuration).Wait();
        }
    app.UseMigrationsEndPoint();
}
else
{

    app.UseExceptionHandler("/Home/Error");
    
    app.UseHsts();
}
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(builder.Environment.ContentRootPath, "node_modules")),
        RequestPath = "/node"
    });
app.UseAuthentication();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute("externallogin", "account/externallogin", new { controller = "Account", action = "ExternalLogin" });
app.MapControllerRoute("admin", "{controller=Admin}/{action=Index}/{id?}");
app.MapControllerRoute("cart", "{controller=Cart}/{action=Index}/{id?}");
app.MapControllerRoute("account", "{controller=Account}/{action=Index}/{id?}");
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "favori",
    pattern: "Account/Favori",
    defaults: new { controller = "Account", action = "Favori" }
);


app.MigrateDatabase().Run();
