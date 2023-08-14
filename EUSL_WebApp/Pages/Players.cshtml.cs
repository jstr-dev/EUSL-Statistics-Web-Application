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
    public class PlayersModel : PageModel
    {
        private readonly EuslContext context;

        public PlayersModel(EuslContext context)
        {
            this.context = context;
        }

        public IEnumerable<Models.Player> Player { get;set; } = default!;

        public async Task OnGetAsync()
        {
            this.Player = await this.context.Players
                .Include(player => player.Nationality).ToListAsync();
        }
    }
}
