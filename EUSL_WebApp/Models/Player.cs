using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Player
{
    public int PlayerId { get; set; }

    public int SlapshotId { get; set; }

    public int NationalityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual Nationality Nationality { get; set; } = null!;

    public virtual ICollection<PlayerToTeam> PlayerToTeams { get; set; } = new List<PlayerToTeam>();

    public virtual ICollection<ResultLine> ResultLines { get; set; } = new List<ResultLine>();
}
