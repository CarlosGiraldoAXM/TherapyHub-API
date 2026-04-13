using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class NoteCategories
{
    public int Id { get; set; }

    public int CompanyId { get; set; }

    public int CreatedByActorId { get; set; }

    public int SectionId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}
