using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class UserTypeMenus
{
    public int Id { get; set; }

    public int UserTypeId { get; set; }

    public int MenuId { get; set; }

    public DateTime AssignedAt { get; set; }
}
