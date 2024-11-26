using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using Info.Pages.Model;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Info.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserDbContext _context;

        public RegisterModel(UserDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(50, ErrorMessage = "Username must be between 3 and 50 characters.", MinimumLength = 3)]
            public string Username { get; set; }

            [Required]
            [EmailAddress(ErrorMessage = "Invalid email address.")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, ErrorMessage = "Password must be at least 6 characters long.", MinimumLength = 6)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null, string topic = null)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Normalize inputs to lowercase for consistency
            var normalizedUsername = Input.Username.ToLower();
            var normalizedEmail = Input.Email.ToLower();

            // Check if the username already exists
            if (_context.Users.Any(u => u.Username.ToLower() == normalizedUsername))
            {
                ModelState.AddModelError("Input.Username", "Username is already taken.");
                return Page();
            }

            // Check if the email already exists
            if (_context.Users.Any(u => u.Email.ToLower() == normalizedEmail))
            {
                ModelState.AddModelError("Input.Email", "An account with this email already exists.");
                return Page();
            }

            // Create and save the new user
            var user = new AppUser
            {
                Username = Input.Username,
                Email = Input.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password) // Hash the password
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Redirect to the login page with returnUrl and topic
            TempData["SuccessMessage"] = "Registration successful. Please log in to continue.";
            return RedirectToPage("/Login", new { returnUrl, topic });
        }


    }
}
