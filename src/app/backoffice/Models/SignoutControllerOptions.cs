using Microsoft.AspNetCore.Mvc;

namespace Conventions.BackOffice.Models
{
    public class SignoutControllerOptions
    {
        public string Authority { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
    }
}
