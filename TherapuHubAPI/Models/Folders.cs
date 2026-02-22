using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Folders
{
    public int Id { get; set; }

    public byte EntityTypeId { get; set; }

    public int? EntityId { get; set; }

    public int? ParentFolderId { get; set; }

    public string Name { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
