using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Result
{
    public int ResultId { get; set; }

    public int FixtureId { get; set; }

    public DateOnly? DatePlayed { get; set; }

    public int? IsOvertime { get; set; }

    public int? IsForfeit { get; set; }

    public int Winner { get; set; }

    public int HomeScore { get; set; }

    public int AwayScore { get; set; }

    public int? SeriesNumber { get; set; }

    public virtual Fixture Fixture { get; set; } = null!;

    public virtual ICollection<ResultLine> ResultLines { get; set; } = new List<ResultLine>();

    public virtual Team WinnerNavigation { get; set; } = null!;
}
