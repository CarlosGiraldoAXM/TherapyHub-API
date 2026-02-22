using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class StaffTimeOff
{
    public long Id { get; set; }

    public int StaffId { get; set; }

    public byte TimeOffTypeId { get; set; }

    public byte StatusId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Reason { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }
}
