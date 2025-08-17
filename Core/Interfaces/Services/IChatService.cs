//using Core.DTOs.ChatDTOs;
//using Core.DTOs;

//namespace Core.Interfaces.Services
//{
//    public interface IChatService
//    {
//        // Chat Room Management
//        Task<GeneralResponse<ChatRoomDTO>> CreateChatRoomAsync(ChatRoomCreateDTO dto);
//        Task<GeneralResponse<ChatRoomDTO>> GetChatRoomByIdAsync(string id);
//        Task<GeneralResponse<IEnumerable<ChatRoomDTO>>> GetChatRoomsByUserAsync(string userId);
//        Task<GeneralResponse<ChatRoomDTO>> GetChatRoomByServiceAsync(string serviceType, string serviceId);
//        Task<GeneralResponse<ChatRoomDTO>> UpdateChatRoomAsync(string id, ChatRoomUpdateDTO dto);
//        Task<GeneralResponse<bool>> DeleteChatRoomAsync(string id);
        
//        // Message Management
//        Task<GeneralResponse<ChatMessageDTO>> SendMessageAsync(ChatMessageCreateDTO dto);
//        Task<GeneralResponse<ChatMessageDTO>> GetMessageByIdAsync(string id);
//        Task<GeneralResponse<IEnumerable<ChatMessageDTO>>> GetMessagesByChatRoomAsync(string chatRoomId, int page = 1, int pageSize = 50);
//        Task<GeneralResponse<ChatMessageDTO>> UpdateMessageAsync(string id, ChatMessageUpdateDTO dto);
//        Task<GeneralResponse<bool>> DeleteMessageAsync(string id);
//        Task<GeneralResponse<bool>> MarkMessageAsReadAsync(string messageId, string userId);
//        Task<GeneralResponse<bool>> MarkAllMessagesAsReadAsync(string chatRoomId, string userId);
        
//        // Participant Management
//        Task<GeneralResponse<ChatParticipantDTO>> AddParticipantAsync(ChatParticipantCreateDTO dto);
//        Task<GeneralResponse<ChatParticipantDTO>> RemoveParticipantAsync(string chatRoomId, string userId);
//        Task<GeneralResponse<ChatParticipantDTO>> UpdateParticipantStatusAsync(string chatRoomId, string userId, ChatParticipantUpdateDTO dto);
//        Task<GeneralResponse<IEnumerable<ChatParticipantDTO>>> GetParticipantsByChatRoomAsync(string chatRoomId);
        
//        // Real-time Features
//        Task<GeneralResponse<bool>> SetTypingStatusAsync(string chatRoomId, string userId, bool isTyping);
//        Task<GeneralResponse<int>> GetUnreadMessageCountAsync(string userId);
//        Task<GeneralResponse<IEnumerable<ChatRoomDTO>>> GetRecentChatsAsync(string userId, int limit = 10);
//    }
//} 