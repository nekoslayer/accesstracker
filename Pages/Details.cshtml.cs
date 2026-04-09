using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly JsonStorageService _storage;

    public DetailsModel(JsonStorageService storage)
    {
        _storage = storage;
    }

    public Submission? Submission { get; set; }

    public IActionResult OnGet([FromQuery] Guid? id)
    {
        if (id == null)
            return RedirectToPage("/Dashboard");

        Submission = _storage.GetById(id.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }
}
