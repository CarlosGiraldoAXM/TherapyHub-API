using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Clients
{
    public int Id { get; set; }

    public string ClientCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public DateOnly? BirthDate { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int ProgramId { get; set; }

    public string? GuardianName { get; set; }

    public int ClientStatusId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CompanyId { get; set; }
}
