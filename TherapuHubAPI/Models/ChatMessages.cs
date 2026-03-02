using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class ChatMessages
{
    public long Id { get; set; }

    public int ChatId { get; set; }

    public int SenderUserId { get; set; }

    public string MessageText { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsEdited { get; set; }

    public bool IsDeleted { get; set; }

    public int? DeleteUserId { get; set; }

    public DateTime? DeletedAt { get; set; }
}
