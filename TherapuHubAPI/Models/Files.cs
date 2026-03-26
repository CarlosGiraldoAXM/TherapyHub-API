using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Files
{
    public long Id { get; set; }

    public int? FolderId { get; set; }

    public string FileName { get; set; } = null!;

    public string BlobPath { get; set; } = null!;

    public int UploadedByActorId { get; set; }

    public DateTime UploadedAt { get; set; }

    public int? OwnerActorId { get; set; }

    public int? FilesTypeId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedActorId { get; set; }
}
