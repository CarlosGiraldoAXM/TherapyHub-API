using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class CompanyChats
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
