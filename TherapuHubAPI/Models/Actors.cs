using System;
using System.Collections.Generic;

namespace TherapuHubAPI.Models;

public partial class Actors
{
    public int Id { get; set; }

    public string ActorType { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int CompanyId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByActorId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Actors? DeletedByActor { get; set; }

    public virtual ICollection<Actors> InverseDeletedByActor { get; set; } = new List<Actors>();
}
