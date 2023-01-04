using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.DAL.Entities.Identity;

namespace Talabat.DAL.Identity
{
    public class AppIdentityDbContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new AppUser()
                {
                    DisplayName = "Mohamed Mohanad",
                    UserName = "Honda99",
                    Email = "mohamedmohanad0852@gmail.com",
                    PhoneNumber = "01143475759",
                    Address = new UserAddress()
                    {
                        FirstName = "Mohamed",
                        LastName = "Mohanad",
                        Country = "Egypt",
                        City = "Giza",
                        Street = "1 Tahrir St.",
                        ZipCode = "24488",
                    },
                };
                await userManager.CreateAsync(user, "Pa$$w0rd");
            }
        }
    }
}
