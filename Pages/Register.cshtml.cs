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
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
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
