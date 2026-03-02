using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Companies
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? TaxId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserLimit { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeleteUserId { get; set; }

    public DateTime? DeletedAt { get; set; }
}
