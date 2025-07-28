using Core.Enums.Attributes;
namespace Core.Enums
{
    public enum RoleType
    {
        [StringValue("Customer")]
        Customer,
        [StringValue("Admin")]
        Admin,
        [StringValue("TechCompany")]
        TechCompany,
        [StringValue("DeliveryPerson")]
        DeliveryPerson
    }
}
