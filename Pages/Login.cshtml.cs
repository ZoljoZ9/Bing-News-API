using Info.Pages.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Debug.WriteLine("Login attempt");

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("Model state invalid");
                return Page();
            }

            var user = _context.Users.SingleOrDefault(u => u.Username == Input.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(Input.Password, user.PasswordHash))
            {
                Debug.WriteLine("Login successful");

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    Debug.WriteLine("Redirecting to ReturnUrl: " + returnUrl);
                    return Redirect(returnUrl);
                }
                else
                {
                    Debug.WriteLine("Redirecting to Dashboard");
                    return RedirectToPage("/Dashboard");
                }
            }

            Debug.WriteLine("Invalid login attempt");
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return Page();
        }


    }
}
