using Cukcuk.Core.DTOs;
using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Cukcuk.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Cukcuk.Infrastructure.Repositories
{
    public class MessageRepository(ApplicationDbContext dbContext) : IMessageRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        public async Task Create(Message message)
        {
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(IEnumerable<Message> messages, int totalPages)> GetConversation(string userId1, string userId2, int pageNumber, int skipItem)
        {
            var query = _dbContext.Messages
                                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) || (m.SenderId == userId2 && m.ReceiverId == userId1));

            var totalMessages = await query.CountAsync();

            var totalPage = (int)Math.Ceiling(totalMessages / 20.0);

            var messages = await query.OrderByDescending(m => m.CreatedAt)
                                    .Skip((pageNumber - 1) * 20 + skipItem)
                                    .Take(20)
                                    .Include(m => m.Sender)
                                    .Include(m => m.Receiver)
                                    .ToListAsync();

            return (messages, totalPage);
        }

        public async Task<IEnumerable<UserMessage>> GetUserToChat(string userId)
        {
            var otherUsers = await _dbContext.Users.Where(u => u.Id != userId).ToListAsync();

            var result = otherUsers.Select(u =>
            {
                var latestMessage = _dbContext.Messages
                    .Where(m => (m.SenderId == userId && m.ReceiverId == u.Id) ||
                                (m.SenderId == u.Id && m.ReceiverId == userId))
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                var latestMessageUserRead = _dbContext.Messages
                    .Where(m => m.SenderId == u.Id && m.ReceiverId == userId && m.Status)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefault();

                var totalMessageUnRead = 0;
                if (latestMessage != null && latestMessage.SenderId == userId) totalMessageUnRead = 0;
                else
                {
                    if (latestMessageUserRead == null)
                    {
                        totalMessageUnRead = _dbContext.Messages.Where(m => m.SenderId == u.Id && m.ReceiverId == userId).Count();
                    }
                    else
                    {
                        totalMessageUnRead = _dbContext.Messages.Where(m => m.SenderId == u.Id && m.ReceiverId == userId && m.CreatedAt > latestMessageUserRead.CreatedAt).Count();
                    }
                }

                return new UserMessage
                {
                    UserId = u.Id,
                    UserName = u.UserName,
                    MessageContent = latestMessage?.Content,
                    MessageCreatedAt = latestMessage?.CreatedAt,
                    MessageStatus = latestMessage?.Status,
                    TotalMessageUnRead = totalMessageUnRead,
                    Type = latestMessage != null && latestMessage.SenderId == userId,
                };
            })
            .OrderByDescending(um => um.MessageCreatedAt) 
            .ToList();

            return result;
        }

        public async Task ReadMessage(Guid id)
        {
            Console.WriteLine("doc tin nhan");
            var msg = await _dbContext.Messages.Where(m => m.MessageId == id).FirstOrDefaultAsync();

            msg.Status = true;

            await _dbContext.SaveChangesAsync();
        }
    }
}
