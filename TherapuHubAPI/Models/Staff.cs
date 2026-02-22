using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Staff
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public byte StatusId { get; set; }

    public DateOnly ContractDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
