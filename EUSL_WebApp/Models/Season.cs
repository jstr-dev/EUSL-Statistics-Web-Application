using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Season
{
    public int SeasonId { get; set; }

    public int Num { get; set; }

    public int Division { get; set; }

    public virtual ICollection<Fixture> Fixtures { get; set; } = new List<Fixture>();

    public virtual ICollection<PlayerToTeam> PlayerToTeams { get; set; } = new List<PlayerToTeam>();
}
