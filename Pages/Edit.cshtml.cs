using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

[Authorize]
public class EditModel : PageModel
{
    private readonly JsonStorageService _storage;

    public EditModel(JsonStorageService storage)
    {
        _storage = storage;
    }

    [BindProperty]
    public Submission? Submission { get; set; }

    public string? SuccessMessage { get; set; }

    public IActionResult OnGet([FromQuery] Guid? id)
    {
        if (id == null)
            return RedirectToPage("/Dashboard");

        Submission = _storage.GetById(id.Value);
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Submission == null)
            return RedirectToPage("/Dashboard");

        if (string.IsNullOrWhiteSpace(Submission.FullName) ||
            string.IsNullOrWhiteSpace(Submission.Username) ||
            string.IsNullOrWhiteSpace(Submission.SystemName) ||
            string.IsNullOrWhiteSpace(Submission.BusinessReason) ||
            string.IsNullOrWhiteSpace(Submission.SecondaryContactName) ||
            string.IsNullOrWhiteSpace(Submission.UnavailabilityPlan))
        {
            ModelState.AddModelError("", "Please fill in all required fields.");
            return Page();
        }

        var updated = _storage.Update(Submission);
        if (!updated)
        {
            ModelState.AddModelError("", "Record not found. It may have been deleted.");
            return Page();
        }

        SuccessMessage = "Record updated successfully.";
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }
}
