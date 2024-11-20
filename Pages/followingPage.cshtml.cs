using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Info.Pages
{
    public class followingPageModel : PageModel
    {
        public string Username { get; set; }

        public void OnGet()
        {
        }
    }
}
