using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Info.Pages.Model
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        public string Username { get; set; }

        public void OnGet()
        {
            Username = User.Identity.Name;
        }
    }

}
