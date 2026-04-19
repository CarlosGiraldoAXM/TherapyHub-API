using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class LibraryItemFiles
{
    public int Id { get; set; }

    public int LibraryItemId { get; set; }

    public string FileName { get; set; } = null!;

    public string BlobPath { get; set; } = null!;

    public string ContentType { get; set; } = null!;

    public long FileSize { get; set; }

    public int UploadedByActorId { get; set; }

    public DateTime UploadedAt { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByActorId { get; set; }

    public virtual LibraryItems LibraryItem { get; set; } = null!;
}
