using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EUSL_WebApp;
using EUSL_WebApp.Models;

namespace EUSL_WebApp.Pages
{
    public class TeamsModel : PageModel
    {
        private readonly EuslContext context;

        public TeamsModel(EuslContext context)
        {
            this.context = context;
        }

        public IEnumerable<Models.Team> Team { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (context.Teams != null)
            {
                Team = await context.Teams.ToListAsync();
            }
        }
    }
}
