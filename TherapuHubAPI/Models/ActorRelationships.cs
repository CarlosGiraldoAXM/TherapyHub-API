using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class ActorRelationships
{
    public long Id { get; set; }

    public int SourceActorId { get; set; }

    public int TargetActorId { get; set; }

    public DateTime CreatedAt { get; set; }
}
