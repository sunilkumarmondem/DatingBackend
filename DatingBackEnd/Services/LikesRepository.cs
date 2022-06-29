using DatingBackEnd.Data;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DatingBackEnd
{
    public class LikesRepository : ILikesRepository
    {
        private readonly DataContext _context;
        public LikesRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId, likedUserId);
        }


        //list of users, that this user was liked 
        public async Task<IEnumerable<LikeDTO>> GetUserLiked( int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            
                likes = likes.Where(like => like.SourceUserId == userId);
                users = likes.Select(like => like.LikedUser);
           

            var userAge = new AppUser();


            return await users.Select(user => new LikeDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.Age,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }

        //list of users, that this user was liked 
        public async Task<IEnumerable<LikeDTO>> GetUserLikedby( int userId)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

 
                likes = likes.Where(like => like.LikedUserId == userId);
                users = likes.Select(like => like.SourceUser);
            

            var userAge = new AppUser();


            return await users.Select(user => new LikeDTO
            {
                Username = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.Age,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url,
                City = user.City,
                Id = user.Id
            }).ToListAsync();
        }


        //list of users that this user liked
        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users
               .Include(x => x.LikedUsers)
               .FirstOrDefaultAsync(x => x.Id == userId);
        }
    }
}
