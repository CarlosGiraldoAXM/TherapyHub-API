using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class MessageReads
{
    public long Id { get; set; }

    public long MessageId { get; set; }

    public int UserId { get; set; }

    public DateTime ReadAt { get; set; }
}
