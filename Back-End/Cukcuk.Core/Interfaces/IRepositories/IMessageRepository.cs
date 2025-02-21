using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;

namespace Cukcuk.Core.Interfaces.IRepositories
{
    public interface IMessageRepository
    {
        Task Create(Message message);
        
        Task<(IEnumerable<Message> messages, int totalPages)> GetConversation(string senderId, string receiverId, int pageNumber, int skipItem);

        Task<IEnumerable<UserMessage>> GetUserToChat(string userId);

        Task ReadMessage(Guid id);

    }
}
