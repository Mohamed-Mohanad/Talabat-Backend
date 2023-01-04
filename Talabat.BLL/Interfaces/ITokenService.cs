using Microsoft.AspNetCore.Identity;
using Talabat.DAL.Entities.Identity;

namespace Talabat.BLL.Interfaces
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser user, UserManager<AppUser> userManager);
    }
}
