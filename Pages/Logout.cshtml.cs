using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[Authorize]
public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnGetAsync()
    {
        // Sign out the user
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Clear the session
        HttpContext.Session.Clear();

        // Redirect to the login or home page
        return RedirectToPage("/Index");
    }
}
