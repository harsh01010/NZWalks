using Microsoft.AspNetCore.Identity;

namespace NZWalks.API.Repositories
{
    public interface ITokenRepository
    {

        public string CreateJWTToken(IdentityUser user, List<string> roles);
        
    }
}
