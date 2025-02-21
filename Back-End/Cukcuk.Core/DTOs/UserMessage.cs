using Cukcuk.Core.Auth;
namespace Cukcuk.Core.DTOs
{
    public class UserMessage
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string MessageContent { get; set; } = string.Empty;
        public DateTime? MessageCreatedAt { get; set; }
        public bool? MessageStatus { get; set; }
        public int TotalMessageUnRead { get; set; }
        public bool Type { get; set; }
    }
}
