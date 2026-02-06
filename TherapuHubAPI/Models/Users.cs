using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Users
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public int UserTypeId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int CompanyId { get; set; }

    public bool MustResetPassword { get; set; }
}
