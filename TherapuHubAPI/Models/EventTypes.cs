using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class EventTypes
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Color { get; set; }

    public string? Icon { get; set; }

    public bool IsActive { get; set; }
}
