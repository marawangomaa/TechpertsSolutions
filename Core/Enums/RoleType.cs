using Core.Enums.Attributes;

namespace Core.Enums
{
    public enum RoleType
    {
        [StringValue("Customer")]
        Customer = 0,
        
        [StringValue("Admin")]
        Admin = 1,
        
        [StringValue("TechCompany")]
        TechCompany = 2,
        
        [StringValue("DeliveryPerson")]
        DeliveryPerson = 3
    }
}
