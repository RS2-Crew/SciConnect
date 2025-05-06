using IdentityServer.Entities;
using IdentityService.DTOs;

namespace IdentityService.Services
{
    public interface IAuthenticationService
    {
        Task<User?> ValidateUser(UserCredentialsDTO userCredentials);
        Task<AuthenticationModel> CreateAuthenticationModel(User user);

        Task RemoveRefreshToken(User user, string refreshToken);
    }
}
