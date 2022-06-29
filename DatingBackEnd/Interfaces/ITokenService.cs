using DatingBackEnd.Entities;

namespace DatingBackEnd.Interfaces
{
    public interface ITokenService
    {
        string createToken(AppUser user);

    }
}
