using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Team
{
    public int TeamId { get; set; }

    public string Name { get; set; } = null!;

    public string ShortName { get; set; } = null!;

    public virtual ICollection<Fixture> FixtureAwayTeams { get; set; } = new List<Fixture>();

    public virtual ICollection<Fixture> FixtureHomeTeams { get; set; } = new List<Fixture>();

    public virtual ICollection<PlayerToTeam> PlayerToTeams { get; set; } = new List<PlayerToTeam>();

    public virtual ICollection<ResultLine> ResultLines { get; set; } = new List<ResultLine>();

    public virtual ICollection<Result> Results { get; set; } = new List<Result>();
}
