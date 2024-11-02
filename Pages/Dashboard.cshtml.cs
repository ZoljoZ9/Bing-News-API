using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace Info.Pages
{
    [Authorize] // Ensure that only logged-in users can access this page
    public class DashboardModel : PageModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        public void OnGet()
        {
            // Retrieve the logged-in user's username and email from their claims
            Username = User.FindFirstValue(ClaimTypes.Name);
            Email = User.FindFirstValue(ClaimTypes.Email);
        }
    }
}
