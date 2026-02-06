using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class EventRecurrence
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public string RecurrenceType { get; set; } = null!;

    public int Interval { get; set; }

    public int? DayOfWeek { get; set; }

    public DateTime? EndDate { get; set; }
}
