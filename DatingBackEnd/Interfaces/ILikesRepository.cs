using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;

namespace DatingBackEnd.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);

        Task<IEnumerable<LikeDTO>> GetUserLiked(int userId);
        Task<IEnumerable<LikeDTO>> GetUserLikedby( int userId);
    }
}
