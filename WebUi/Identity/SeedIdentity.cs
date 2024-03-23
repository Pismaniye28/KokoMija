using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bussines.Abstract;
using Microsoft.AspNetCore.Identity;

namespace WebUi.Identity
{
    public static class SeedIdentity
    {
        public static async Task Seed(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,ICartService cartService,IConfiguration configuration)
        {

            var roles = configuration.GetSection("Data:Roles").GetChildren().Select(x=>x.Value).ToArray();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var users = configuration.GetSection("Data:Users");

            foreach (var section in users.GetChildren())
            {
                var username = section.GetValue<string>("username");
                var password = section.GetValue<string>("password");
                var email = section.GetValue<string>("email");
                var role = section.GetValue<string>("role");
                var firstName = section.GetValue<string>("firstName");
                var lastName = section.GetValue<string>("lastName");
                var ProfileImg = section.GetValue<string>("ProfileImg");

                if(await userManager.FindByNameAsync(username)==null)
                {
                    var user = new User()
                    {
                        UserName = username,
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        EmailConfirmed = true,
                        ProfileImg = ProfileImg
                    };

                    var result = await userManager.CreateAsync(user,password);
                    if(result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user,role);
                        cartService.InitializeCart(user.Id);
                    }
                }
            }
        }

    }
}