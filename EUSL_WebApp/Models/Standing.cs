using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

[Keyless]
public partial class Standing
{
    public int TeamID { get; set; }

    public string TeamName { get; set; }

    public int Matches { get; set; }

    public int Points { get; set; }

    public int Wins { get; set; }

    public int OtWins { get; set; }

    public int OtLosses { get; set; }

    public int Losses { get; set; }

    public int Forfeits { get; set; }

    public int GoalsFor { get; set; }

    public int GoalsAgainst { get; set; }

    public int GoalDifference { get; set; }

    public virtual Team Team { get; set; } = null!;
}
