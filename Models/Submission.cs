namespace AccessTracker.Models;

public class Submission
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }

    // User
    public string FullName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;

    // System
    public string SystemName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;

    // Access
    public string AccessType { get; set; } = string.Empty;
    public string AccessTypeOther { get; set; } = string.Empty;
    public string AuthenticationMethod { get; set; } = string.Empty;

    // Business Context
    public string BusinessReason { get; set; } = string.Empty;
    public string BusinessProcess { get; set; } = string.Empty;
    public string ImpactIfLost { get; set; } = string.Empty;

    // Continuity
    public string SecondaryContactName { get; set; } = string.Empty;
    public string SecondaryContactEmail { get; set; } = string.Empty;
    public string UnavailabilityPlan { get; set; } = string.Empty;

    // Lifecycle
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool TemporaryAccess { get; set; }

    // Extra
    public string CriticalityLevel { get; set; } = string.Empty;
    public string BackupAccessMethod { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
