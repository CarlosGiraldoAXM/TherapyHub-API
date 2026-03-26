using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Users
{
    public int Id { get; set; }

    public string PasswordHash { get; set; } = null!;

    public int UserTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public bool MustResetPassword { get; set; }

    public int ActorId { get; set; }
}
