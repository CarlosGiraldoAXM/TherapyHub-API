using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class StaffDocuments
{
    public long Id { get; set; }

    public int StaffId { get; set; }

    public byte DocumentTypeId { get; set; }

    public string BlobUrl { get; set; } = null!;

    public string FileName { get; set; } = null!;

    public DateTime UploadedAt { get; set; }

    public bool IsActive { get; set; }
}
