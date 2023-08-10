using EUSL_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;

namespace EUSL_WebApp.Pages
{
    public class StandingsModel : PageModel
    {
        private readonly EuslContext context;
        public IEnumerable<Standing> Standings { get; set;} = default!;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public Season CurrentSeason { get; set; } = default!;

        public StandingsModel(EuslContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            this.Seasons = context.Seasons.ToList();

            // Get the current pro season
            var current = await context.Seasons.Where(season => season.Division == 1).OrderByDescending(season => season.Division).FirstAsync();
            if (current != null)
            {
                this.CurrentSeason = current;
            } else
            {
                return NotFound();
            }

            var standings = await context.GetStandings(this.CurrentSeason);
            this.Standings = standings;

            return Page();
        }
    }
}
