using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Menus
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Route { get; set; } = null!;

    public string? Icon { get; set; }

    public int? SortOrder { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? ParentId { get; set; }

    public bool IsSystem { get; set; }
}
