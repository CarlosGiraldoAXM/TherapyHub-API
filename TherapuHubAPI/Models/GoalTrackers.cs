using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class GoalTrackers
{
    public long Id { get; set; }

    public int OwnerActorId { get; set; }

    public int CreatedByActorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? DeleteAt { get; set; }

    public int? DeletedByActorId { get; set; }
}
