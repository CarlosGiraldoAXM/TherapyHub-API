using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Programs
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}
