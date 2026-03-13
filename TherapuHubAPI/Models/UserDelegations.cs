using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class UserDelegations
{
    public int Id { get; set; }

    public int OwnerUserId { get; set; }

    public int DelegateUserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
