using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum MessageType
    {
        [StringValue("Text")]
        Text = 0,
        
        [StringValue("Image")]
        Image = 1,
        
        [StringValue("File")]
        File = 2,
        
        [StringValue("System")]
        System = 3
    }
} 