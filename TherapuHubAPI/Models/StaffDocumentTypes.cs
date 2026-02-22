using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class StaffDocumentTypes
{
    public byte Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}
