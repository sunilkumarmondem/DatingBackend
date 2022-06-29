using AutoMapper;
using DatingBackEnd.Data;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DatingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;   
            
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
        {
            if (await UserExists(registerDTO.Username)) return BadRequest("Username is taken");

            var user = _mapper.Map<AppUser>(registerDTO);

            using var hmac = new HMACSHA512();


            user.UserName = registerDTO.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
            user.PasswordSalt = hmac.Key;
            
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _tokenService.createToken(user)
            };

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDTO.UserName);

            if (user == null) return Unauthorized("Invalid userName");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");

            }
            return new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Token = _tokenService.createToken(user)
            };

        }

        private async Task<bool> UserExists(string userName)
        {
            return await _context.Users.AnyAsync(x => x.UserName == userName.ToLower());
        }

    }
}
