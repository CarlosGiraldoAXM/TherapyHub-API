using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Files
{
    public long Id { get; set; }

    public byte EntityTypeId { get; set; }

    public int? EntityId { get; set; }

    public int? FolderId { get; set; }

    public byte? FileTypeId { get; set; }

    public string BlobUrl { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public bool IsActive { get; set; }
}
