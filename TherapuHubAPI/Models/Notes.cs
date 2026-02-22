using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Notes
{
    public long Id { get; set; }

    public int CompanyId { get; set; }

    public int CreatedByUserId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte PriorityId { get; set; }

    public byte NoteTypeId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
