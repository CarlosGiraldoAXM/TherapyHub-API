using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class NoteModules
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Path { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ICollection<NoteSections> NoteSections { get; set; } = new List<NoteSections>();
}
