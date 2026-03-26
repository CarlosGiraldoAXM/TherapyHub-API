using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class FilesTypes
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? IsActive { get; set; }
}
