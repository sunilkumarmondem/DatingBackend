using AutoMapper;
using DatingBackEnd.Data;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DatingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppUserController:ControllerBase
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppUserController(DataContext context, IMapper mapper, IPhotoService photoService)
        {
            _context = context;
            _mapper = mapper;
            _photoService = photoService;
            _httpContextAccessor = new HttpContextAccessor();
        }

        [HttpGet]
    
        public async Task<ActionResult<IEnumerable<AppUserDTO>>> GetAppUserS()
        {
           // var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
           var userName = User?.Identity?.Name;
            var users = await _context.Users.Include((p) => p.Photos).Where(p => p.UserName!= userName).ToListAsync();
            var usersToReturn = _mapper.Map<IEnumerable<AppUserDTO>>(users);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUserDTO>> GetAppUser(int id)
        {
            var appUser = await _context.Users.Include((p) => p.Photos).SingleOrDefaultAsync(p =>p.Id == id);
            var appUserToReturn = _mapper.Map<AppUserDTO>(appUser);

            if (appUserToReturn == null)
            {
                return NotFound();
            }

            return appUserToReturn;
        }


        [HttpGet("{userName}/details")]
        public async Task<ActionResult<AppUserDTO>> GetAppUsername(string userName)
        {
            var appUser = await _context.Users.Include((p) => p.Photos).SingleOrDefaultAsync(x => x.UserName == userName);
            var appUserToReturn = _mapper.Map<AppUserDTO>(appUser);

            if (appUserToReturn == null)
            {
                return NotFound();
            }

            return appUserToReturn;
        }

        [HttpPost]
        public async Task<ActionResult<AppUser>> PostAppUser(AppUser appUser)
        {
            _context.Users.Add(appUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppUser", new { id = appUser.Id }, appUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppUser(int id)
        {
            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(appUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut]
        public async Task<ActionResult> UpdateAppUser(MemberUpdateDto memberUpdateDto)
        {
            var userName = User?.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
             
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == userName);
            if(user!= null)
            {
                user.Introduction = memberUpdateDto.Introduction;
                user.LookingFor = memberUpdateDto.LookingFor;
                user.Interests = memberUpdateDto.Interests;
                user.City = memberUpdateDto.City;
                user.Country = memberUpdateDto.Country;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok(user);
            }
            return BadRequest("Bad request to server");
            

        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var userName = User?.Identity?.Name;
            var user =  await _context.Users.Include((p) => p.Photos).SingleOrDefaultAsync(x => x.UserName == userName);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var lastPhotoId = _context.Photos.OrderByDescending(a => a.Id).First();
            var newPhotoId = lastPhotoId.Id + 1;

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
               Id = newPhotoId,
               AppUserId=user.Id
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }

            
            user.Photos.Add(photo);
           
            return Ok(user) ;

        }



        }
}
