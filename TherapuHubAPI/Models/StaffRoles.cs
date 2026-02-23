using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class StaffRoles
{
    public short Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}
