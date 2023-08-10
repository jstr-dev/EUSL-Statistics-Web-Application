using System;
using System.Collections.Generic;

namespace EUSL_WebApp.Models;

public partial class Nationality
{
    public int NationalityId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
