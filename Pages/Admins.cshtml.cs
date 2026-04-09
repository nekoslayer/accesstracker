using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AccessTracker.Models;
using AccessTracker.Services;

namespace AccessTracker.Pages;

[Authorize]
public class AdminsModel : PageModel
{
    private readonly AccountStorageService _accounts;

    public AdminsModel(AccountStorageService accounts)
    {
        _accounts = accounts;
    }

    public List<AdminAccount> Accounts { get; set; } = new();
    public string? Message { get; set; }
    public bool IsError { get; set; }

    [BindProperty] public string NewUsername { get; set; } = string.Empty;
    [BindProperty] public string NewPassword { get; set; } = string.Empty;
    [BindProperty] public string ConfirmPassword { get; set; } = string.Empty;
    [BindProperty] public Guid DeleteId { get; set; }

    public void OnGet()
    {
        Accounts = _accounts.GetAll();
    }

    public IActionResult OnPostAdd()
    {
        if (string.IsNullOrWhiteSpace(NewUsername) || string.IsNullOrWhiteSpace(NewPassword))
        {
            Message = "Username and password are required.";
            IsError = true;
        }
        else if (NewPassword.Length < 6)
        {
            Message = "Password must be at least 6 characters.";
            IsError = true;
        }
        else if (NewPassword != ConfirmPassword)
        {
            Message = "Passwords do not match.";
            IsError = true;
        }
        else if (_accounts.UsernameExists(NewUsername))
        {
            Message = $"Username '{NewUsername}' already exists.";
            IsError = true;
        }
        else
        {
            _accounts.Add(NewUsername, NewPassword);
            Message = $"Admin account '{NewUsername}' created successfully.";
            IsError = false;
        }

        Accounts = _accounts.GetAll();
        return Page();
    }

    public IActionResult OnPostDelete()
    {
        var deleted = _accounts.Delete(DeleteId);
        if (!deleted)
        {
            Message = "Cannot delete the last admin account.";
            IsError = true;
        }
        else
        {
            Message = "Admin account deleted.";
            IsError = false;
        }

        Accounts = _accounts.GetAll();
        return Page();
    }

    public async Task<IActionResult> OnPostLogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Login");
    }
}
