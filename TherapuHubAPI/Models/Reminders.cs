using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Reminders
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int MinutesBefore { get; set; }

    public string Channel { get; set; } = null!;

    public bool IsActive { get; set; }
}
