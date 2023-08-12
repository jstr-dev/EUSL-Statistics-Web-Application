using EUSL_WebApp.Models;
using EUSL_WebApp.Pages.Team;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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

        public async Task<IActionResult> OnGetAsync(int? season)
        {
            this.Seasons = context.Seasons.ToList();

            var current = (season != null) ? await context.Seasons.Where(sz => sz.SeasonId == season).FirstAsync() : await context.Seasons.Where(sz => sz.Division == 1).OrderByDescending(sz => sz.Num).FirstAsync();
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

        public async Task<PartialViewResult> OnGetUpdateStandingsAsync(int number, int division)
        {
            System.Diagnostics.Debug.WriteLine("Hey " + number + " " + division);

            var szn = await this.context.Seasons.Where(sz => sz.Division == division && sz.Num == number).FirstAsync();
            var standings = await this.context.GetStandings(szn);

            return new PartialViewResult
            {
                ViewName = "_StandingPartial",
                ViewData = new ViewDataDictionary<IEnumerable<Standing>>(ViewData, standings)
            };
        }
    }
}
