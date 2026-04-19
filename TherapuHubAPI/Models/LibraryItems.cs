using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class LibraryItems
{
    public int Id { get; set; }

    public byte CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public string? Barriers { get; set; }

    public string? Measurement { get; set; }

    public string? Functions { get; set; }

    public string? Topography { get; set; }

    public string? Definition { get; set; }

    public string? Objective { get; set; }

    public string? Procedures { get; set; }

    public string? TeachingMaterials { get; set; }

    public int CreatedByActorId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByActorId { get; set; }

    public virtual LibraryCategories Category { get; set; } = null!;

    public virtual ICollection<LibraryItemFiles> LibraryItemFiles { get; set; } = new List<LibraryItemFiles>();
}
