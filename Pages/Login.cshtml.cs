using Info.Pages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Info.Pages
{
    public class LoginModel : PageModel
    {
        private readonly UserDbContext _context;

        public LoginModel(UserDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            // Check if the user is already authenticated
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string topic = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Normalize the username for comparison
            var normalizedUsername = Input.Username.ToLower();
            var user = _context.Users.FirstOrDefault(u => u.Username.ToLower() == normalizedUsername);

            if (user != null && BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
            {
                // Create claims for the user
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id.ToString()) // Add UserId to claims
        };

                // Create identity and principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // If a topic was provided, save it
                if (!string.IsNullOrEmpty(topic))
                {
                    var followedTopic = new FollowedTopic
                    {
                        UserId = user.Id,
                        Topic = topic
                    };

                    _context.FollowedTopics.Add(followedTopic);
                    await _context.SaveChangesAsync();
                }

                // Redirect to returnUrl or dashboard
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToPage("/Dashboard");
            }

            // If login fails
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return Page();
        }




    }
}