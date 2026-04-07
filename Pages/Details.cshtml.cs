using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

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
}
