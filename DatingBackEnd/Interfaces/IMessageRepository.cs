using DatingBackEnd.DTOs;
using DatingBackEnd.Entities;
using DatingBackEnd.Helpers;

namespace DatingBackEnd.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);

        Task<bool> SaveAllAsync();
    }
}
