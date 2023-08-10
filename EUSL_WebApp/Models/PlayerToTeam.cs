using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class PlayerToTeam
{
    public int TransferId { get; set; }

    public int PlayerId { get; set; }

    public int TeamId { get; set; }

    public int SeasonId { get; set; }

    public int PositionId { get; set; }

    public DateOnly? TransferDate { get; set; }

    public int IsInitial { get; set; }

    public int IsGm { get; set; }

    public int? TransferedTo { get; set; }

    public int? TransferedFrom { get; set; }

    public virtual ICollection<PlayerToTeam> InverseTransferedFromNavigation { get; set; } = new List<PlayerToTeam>();

    public virtual ICollection<PlayerToTeam> InverseTransferedToNavigation { get; set; } = new List<PlayerToTeam>();

    public virtual Player Player { get; set; } = null!;

    public virtual PlayerPosition Position { get; set; } = null!;

    public virtual Season Season { get; set; } = null!;

    public virtual Team Team { get; set; } = null!;

    public virtual PlayerToTeam? TransferedFromNavigation { get; set; }

    public virtual PlayerToTeam? TransferedToNavigation { get; set; }
}
