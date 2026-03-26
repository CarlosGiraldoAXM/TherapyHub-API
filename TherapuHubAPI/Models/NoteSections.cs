using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class NoteSections
{
    public int Id { get; set; }

    public int MenuId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}
