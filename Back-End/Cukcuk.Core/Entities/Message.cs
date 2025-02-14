using Cukcuk.Core.Auth;

namespace Cukcuk.Core.Entities
{
    public class Message
    {
        public Guid MessageId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderId { get; set; } = string.Empty;
        public string ReceiverId { get; set; } = string.Empty;
        public ApplicationUser? Sender { get; set; }
        public ApplicationUser? Receiver { get; set; }
    }
}
