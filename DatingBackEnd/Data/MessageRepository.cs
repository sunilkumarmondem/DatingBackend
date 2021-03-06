using AutoMapper;
using AutoMapper.QueryableExtensions;
using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Helpers;
using DatingBackEnd.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DatingBackEnd.Data
{
    public class MessageRepository : IMessageRepository
    {

        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
               .OrderByDescending(m => m.MessageSent)
               .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
               .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username
                    && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username
                    && u.SenderDeleted == false),
                _ => query.Where(u => u.RecipientUsername ==
                    messageParams.Username && u.RecipientDeleted == false && u.DateRead == null)
            };

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername
                && m.Sender.UserName == recipientUsername
                || m.Recipient.UserName == recipientUsername
                && m.Sender.UserName == currentUsername)
                .OrderBy(m => m.MessageSent)
                .ToListAsync();
            
            var unreadMessages = messages.Where(m => m.DateRead == null &&
            m.Recipient.UserName ==currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach(var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }
            }

            await _context.SaveChangesAsync();
            
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
