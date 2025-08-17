using Core.Interfaces;
using TechpertsSolutions.Core.Entities;

namespace Core.Entities
{
    public class PrivateMessage : BaseEntity,IEntity
    {
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
        public AppUser SenderUser { get; set; }
        public AppUser ReceiverUser { get; set; }
        public string SenderRole { get; set; }
        public string ReceiverRole { get; set; }
    }
}
