using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

public class SubmitModel : PageModel
{
    private readonly JsonStorageService _storage;

    public SubmitModel(JsonStorageService storage)
    {
        _storage = storage;
    }

    [BindProperty]
    public Submission Submission { get; set; } = new();

    public string? SuccessMessage { get; set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
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

        Submission.Id = Guid.NewGuid();
        Submission.CreatedAt = DateTime.Now;

        _storage.Add(Submission);

        SuccessMessage = "Your access record has been saved.";
        return Page();
    }
}
