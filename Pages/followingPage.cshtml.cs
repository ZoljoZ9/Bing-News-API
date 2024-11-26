using Microsoft.AspNetCore.Mvc.RazorPages;
using Info.Pages.Model;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Info.Pages
{
    public class followingPageModel : PageModel
    {
        private readonly UserDbContext _dbContext;

        public followingPageModel(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public string Username { get; set; }
        public List<string> FollowedTopics { get; set; } = new List<string>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Redirect to login if the user is not authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Login", new { returnUrl = "/followingPage" });
            }

            // Get the UserId from claims
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToPage("/Error");
            }

            var userId = int.Parse(userIdClaim);

            // Retrieve user from the database and their followed topics
            var user = await _dbContext.Users.FindAsync(userId);

            if (user == null)
            {
                return RedirectToPage("/Error");
            }

            Username = user.Username;

            // Retrieve followed topics
            FollowedTopics = await _dbContext.FollowedTopics
                .Where(ft => ft.UserId == userId)
                .Select(ft => ft.Topic)
                .ToListAsync();

            return Page();
        }
    }
}
