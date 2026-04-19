using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class library_permissions
{
    public int id { get; set; }

    public int actorId { get; set; }

    public bool canCreate { get; set; }

    public bool canEdit { get; set; }

    public bool canDelete { get; set; }

    public int assignedBy { get; set; }

    public DateTime assignedAt { get; set; }
}
