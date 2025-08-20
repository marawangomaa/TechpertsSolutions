using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs
{
    public class PrivateMessageDTO
    {
        public string Id { get; set; }            // from BaseEntity
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public string SenderName { get; set; }    // from AppUser.FullName
        public string ReceiverName { get; set; }  // from AppUser.FullName
        public string MessageText { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
    }
}
