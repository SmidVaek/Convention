namespace Conventions.Identity.Models
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = true;
    }
}
