namespace TechpertsSolutions.Core.DTOs.Login
{
    public class LoginResultDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public IList<string> RoleName { get; set; }
    }
}
