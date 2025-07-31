using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum ParticipantRole
    {
        [StringValue("Customer")]
        Customer = 0,
        
        [StringValue("TechCompany")]
        TechCompany = 1,
        
        [StringValue("DeliveryPerson")]
        DeliveryPerson = 2,
        
        [StringValue("Admin")]
        Admin = 3
    }
} 