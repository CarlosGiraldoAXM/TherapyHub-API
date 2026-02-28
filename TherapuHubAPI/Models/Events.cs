using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Events
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsAllDay { get; set; }

    public int EventTypeId { get; set; }

    public bool IsGlobal { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int CompanyId { get; set; }

    public int? StaffId { get; set; }
}
