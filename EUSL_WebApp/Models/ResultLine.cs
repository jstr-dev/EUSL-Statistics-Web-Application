using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class ResultLine
{
    public int ResultLineId { get; set; }

    public int ResultId { get; set; }

    public int PlayerId { get; set; }

    public int TeamId { get; set; }

    public int Score { get; set; }

    public int Goals { get; set; }

    public int PrimaryAssists { get; set; }

    public int SecondaryAssists { get; set; }

    public int Shots { get; set; }

    public int Saves { get; set; }

    public int Passes { get; set; }

    public int Blocks { get; set; }

    public int PostHits { get; set; }

    public int Takeaways { get; set; }

    public int Turnovers { get; set; }

    public int Possession { get; set; }

    public int FaceoffWins { get; set; }

    public int FaceoffLosses { get; set; }

    public int PeriodWins { get; set; }

    public int PeriodLosses { get; set; }

    public int PeriodPlayed()
    {
        return this.PeriodLosses + this.PeriodWins + this.PeriodTies;
    }

    public int PeriodTies { get; set; }

    public int ShutoutFor { get; set; }

    public int ShutoutAgainst { get; set; }

    public int GoalsFor { get; set; }

    public int GoalsAgainst { get; set; }

    public int GameWinningGoals { get; set; }

    public virtual Player Player { get; set; } = null!;

    public virtual Result Result { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;
}
