using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class PlayerPosition
{
    public int PositionId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<PlayerToTeam> PlayerToTeams { get; set; } = new List<PlayerToTeam>();
}
