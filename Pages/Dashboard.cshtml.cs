using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

public class DashboardModel : PageModel
{
    private readonly JsonStorageService _storage;

    public DashboardModel(JsonStorageService storage)
    {
        _storage = storage;
    }

    public List<Submission> Filtered { get; set; } = new();

    [FromQuery] public string? Department { get; set; }
    [FromQuery(Name = "system")] public string? SystemSearch { get; set; }
    [FromQuery(Name = "criticality")] public string? Criticality { get; set; }
    [FromQuery(Name = "expired")] public bool ShowExpired { get; set; }

    public void OnGet()
    {
        var all = _storage.GetAll();

        if (!string.IsNullOrWhiteSpace(Department))
            all = all.Where(s => s.Department == Department).ToList();

        if (!string.IsNullOrWhiteSpace(SystemSearch))
            all = all.Where(s => s.SystemName.Contains(SystemSearch, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(Criticality))
            all = all.Where(s => s.CriticalityLevel == Criticality).ToList();

        if (!ShowExpired)
            all = all.Where(s => !s.EndDate.HasValue || s.EndDate.Value >= DateTime.Today).ToList();

        Filtered = all.OrderByDescending(s => s.CreatedAt).ToList();
    }
}
