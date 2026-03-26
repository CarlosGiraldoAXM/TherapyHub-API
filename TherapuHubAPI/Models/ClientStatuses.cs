using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class ClientStatuses
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
