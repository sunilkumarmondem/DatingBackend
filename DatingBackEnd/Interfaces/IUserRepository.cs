using DatingBackEnd.Entities;

namespace DatingBackEnd.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser> GetUserByUsernameAsync(string username);
    }
}
