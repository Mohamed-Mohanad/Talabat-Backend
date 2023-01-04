using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.DAL.Entities.Identity;

namespace Talabat.API.Extensions
{
    public static class UserManagerExtensions
    {
        public static async Task<AppUser> FindByEmailWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirstValue(ClaimTypes.Email);
            return await userManager.Users.Include(u => u.Address).SingleOrDefaultAsync(x => x.Email == email);
        }
    }
}
