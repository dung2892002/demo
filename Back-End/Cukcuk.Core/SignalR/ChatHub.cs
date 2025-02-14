using Cukcuk.Core.Entities;
using Cukcuk.Core.Interfaces.IRepositories;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Cukcuk.Core.SignalR
{
    public class ChatHub(IMessageRepository messageRepository) : Hub
    {
        private readonly IMessageRepository _repo = messageRepository;

        private static readonly Dictionary<string, List<string>> _connections = new();

        public override Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name;
            var userId = Context.User?.Claims?.FirstOrDefault(static x => x.Type == "Id")?.Value;
            if (userId != null)
            {
                Console.WriteLine("Connected");
                if (!_connections.ContainsKey(userId))
                {
                    _connections[userId] = new List<string>();
                }
                _connections[userId].Add(Context.ConnectionId);
            }
            var onlineUsers = _connections.Keys.ToList();
            Console.WriteLine("danh sach user online: ");
            foreach (var user in onlineUsers)
            {
                Console.WriteLine(user.ToString());
            }
            Clients.All.SendAsync("UserOnlineMessage", userName, onlineUsers);

            return base.OnConnectedAsync();
        }
        
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine("Disconnected");
            var userName = Context.User?.Identity?.Name;
            var userId = Context.User?.Claims?.FirstOrDefault(x => x.Type == "Id")?.Value;
            if (userId != null && _connections.ContainsKey(userId))
            {
                _connections[userId].Remove(Context.ConnectionId);
                if (_connections[userId].Count == 0)
                {
                    _connections.Remove(userId); 
                }
            }
            var onlineUsers = _connections.Keys.ToList();
            Console.WriteLine("danh sach user online: ");
            foreach (var user in onlineUsers)
            {
                Console.WriteLine(user.ToString());
            }
            Clients.All.SendAsync("UserOfflineMessage", onlineUsers);

            return base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.User?.Claims?.FirstOrDefault(static x => x.Type == "Id")?.Value;
            var senderName = Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
            var chatMessage = new Message
            {
                MessageId = Guid.NewGuid(),
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.Now,
                Status = false,
                Content = content
            };

            await _repo.Create(chatMessage);
            await Clients.Caller.SendAsync("MessageSent", receiverId, content);
            if (_connections.ContainsKey(receiverId))
            {
                var connectionIds = _connections[receiverId];
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("ReceiveMessage", senderId, senderName.ToString(), chatMessage);
                }
            }
        }

        public async Task OnTyping(string receiverId)
        {
            var senderId = Context.User?.Claims?.FirstOrDefault(static x => x.Type == "Id")?.Value;
            if (_connections.ContainsKey(receiverId))
            {
                var connectionIds = _connections[receiverId];
                foreach (var connectionId in connectionIds)
                {
                    await Clients.Client(connectionId).SendAsync("SenderTypingMessage", senderId);
                }
            }
        }
    }
}
