using AutoMapper;
using DatingBackEnd.Data;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DatingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LikesController : ControllerBase
    {
       
        private readonly ILikesRepository _likesRepository;
        private readonly IUserRepository _userRepository;
        private readonly DataContext _datacontext;
        public LikesController(DataContext datacontext, ILikesRepository likesRepository, IUserRepository userRepository)
        {
            _datacontext = datacontext;
            _likesRepository = likesRepository;
            _userRepository = userRepository;   
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
            int SourceId = Int32.Parse(sourceUserId);

            var likedUser = await _userRepository.GetUserByUsernameAsync(username);
            var sourceUser = await _likesRepository.GetUserWithLikes(SourceId);

            if (likedUser == null) return NotFound();

            if (sourceUser.UserName == username) return BadRequest("You cannot like yourself");

            var userLike = await _likesRepository.GetUserLike(SourceId, likedUser.Id);

            if(userLike != null) return BadRequest("You already like this user");

            userLike = new UserLike
            {
                SourceUserId = SourceId,
                LikedUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

          _datacontext.SaveChanges();
            return Ok();

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes()
        {
            var sourceUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int SourceId = Int32.Parse(sourceUserId);

            var users = await _likesRepository.GetUserLiked(SourceId);
            return Ok(users);

        }

        [HttpGet("userLikedBy")]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikedBy()
        {
            var sourceUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int SourceId = Int32.Parse(sourceUserId);

            var users = await _likesRepository.GetUserLikedby( SourceId);
            return Ok(users);

        }


    }
}
