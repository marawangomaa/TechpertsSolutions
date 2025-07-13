namespace Core.Enums
{
    public enum RoleType
    {
        [StringValue("Customer")]
        Customer,
        [StringValue("Admin")]
        Admin,
        [StringValue("SaleManager")]
        SaleManager,
        [StringValue("TechManager")]
        TechManager,
        [StringValue("StockControlManager")]
        StockControlManager,
        [StringValue("TechCompany")]
        TechCompany
    }
    [AttributeUsage(AttributeTargets.Field)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }
        public StringValueAttribute(string value) => Value = value;
    }
}
