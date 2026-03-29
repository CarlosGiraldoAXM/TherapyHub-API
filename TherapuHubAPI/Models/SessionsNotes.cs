using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class SessionsNotes
{
    public long Id { get; set; }

    public int RbtActorId { get; set; }

    public int ClientActorId { get; set; }

    public DateTime SessionDate { get; set; }

    public int CreatedByActorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int StatusId { get; set; }

    public string? Notes { get; set; }

    public string? Actions { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByActorId { get; set; }
}
