using NotAlone.Models;

namespace NotAlone.Services.Abstract
{
    public interface IRegistrationService
    {

        Task<string?> signUpUser(UserModel user);
        Task<string?> signInUser(UserModel user);
    }
}
