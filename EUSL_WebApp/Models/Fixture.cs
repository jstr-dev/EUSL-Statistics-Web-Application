using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Fixture
{
    public int FixtureId { get; set; }

    public int HomeTeamId { get; set; }

    public int AwayTeamId { get; set; }

    public int SeasonId { get; set; }

    public int Gameweek { get; set; }

    public int IsPlayoff { get; set; }

    public virtual Team AwayTeam { get; set; } = null!;

    public virtual Team HomeTeam { get; set; } = null!;

    public virtual Result? Result { get; set; }

    public virtual Season Season { get; set; } = null!;
}
