using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class GoalTrackerCategories
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? DisplayOrder { get; set; }

    public bool? IsActive { get; set; }
}
