using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using BCrypt.Net;
using Info.Pages.Model;

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


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Normalize inputs to lowercase for comparison
            var normalizedUsername = Input.Username.ToLower();
            var normalizedEmail = Input.Email.ToLower();

            // Check for duplicate username
            if (_context.Users.Any(u => u.Username.ToLower() == normalizedUsername))
            {
                ModelState.AddModelError("Input.Username", "Username is already taken.");
                return Page();
            }

            // Check for duplicate email
            if (_context.Users.Any(u => u.Email.ToLower() == normalizedEmail))
            {
                ModelState.AddModelError("Input.Email", "An account with this email already exists.");
                return Page();
            }

            // Create a new user
            var user = new AppUser
            {
                Username = Input.Username,
                Email = Input.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Input.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Login");
        }

    }
}
