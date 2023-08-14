using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using EUSL_WebApp;
using EUSL_WebApp.Models;

namespace EUSL_WebApp.Pages.Player
{
    public class indexModel : PageModel
    {
        private readonly EuslContext context;

        public indexModel(EuslContext context)
        {
            this.context = context;
        }

        public EUSL_WebApp.Models.Player Player { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var player = await context.Players.FirstOrDefaultAsync(m => m.PlayerId == id);
            if (player == null)
                return NotFound("Player not found");

            this.Player = player;

            return Page();
        }
    }
}
