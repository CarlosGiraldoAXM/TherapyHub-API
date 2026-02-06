using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class EventUsers
{
    public int Id { get; set; }

    public int EventId { get; set; }

    public int UserId { get; set; }
}
