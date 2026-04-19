using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class LibraryCategories
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<LibraryItems> LibraryItems { get; set; } = new List<LibraryItems>();
}
