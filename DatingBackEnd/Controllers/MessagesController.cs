using AutoMapper;
using DatingBackEnd.Data;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Helpers;
using DatingBackEnd.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly DataContext _datacontext;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository userRepository, DataContext datacontext, IMessageRepository messageRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _datacontext = datacontext;
            _messageRepository = messageRepository;
            _mapper = mapper;   
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = User?.Identity?.Name;

            if (userName == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(userName);
            var recepient = await  _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recepient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recepient,
                SenderUsername = sender.UserName,
                RecipientUsername = recepient.UserName,
                Content = createMessageDto.Content
            };
            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

            return BadRequest("Failed to send message");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery]
            MessageParams messageParams)
        {
            messageParams.Username = User?.Identity?.Name;

            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            //Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
            //    messages.TotalCount, messages.TotalPages);

            return messages;
        }


        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User?.Identity?.Name;
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }
    }
}
