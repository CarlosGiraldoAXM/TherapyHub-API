using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Notes
{
    public long Id { get; set; }

    public int CompanyId { get; set; }

    public int CreatedByActorId { get; set; }

    public int? OwnerActorId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public byte PriorityId { get; set; }

    public byte NoteTypeId { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int MenuId { get; set; }

    public int? SectionId { get; set; }

    public bool IsPrivate { get; set; }

    public int? CategoryId { get; set; }
}
