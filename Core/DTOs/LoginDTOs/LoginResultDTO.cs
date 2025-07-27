namespace TechpertsSolutions.Core.DTOs.LoginDTOs
{
    public class LoginResultDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> RoleName { get; set; }
        public string? CustomerId { get; set; }
        public string? AdminId { get; set; }
        public string? TechCompanyId { get; set; }
        public string? DeliveryPersonId { get; set; }
        public string? CartId { get; set; }
        public string? WishListId { get; set; }
        public string? PCAssemblyId { get; set; }
    }
}
