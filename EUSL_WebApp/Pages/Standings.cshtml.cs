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
            var current = await context.Seasons.Where(season => season.Division == 1).OrderByDescending(season => season.Num).FirstAsync();
            if (current != null)
            {
                this.CurrentSeason = current;
            } else
            {
                return NotFound();
            }

            var standings = await context.GetStandings(this.CurrentSeason);
            this.Standings = standings;

           /* var x = from team in context.Teams
                    from fixture in context.Fixtures
                    where (team.TeamId == fixture.HomeTeamId || team.TeamId == fixture.AwayTeamId)
                    join result in context.Results on fixture.FixtureId equals result.FixtureId
                    join season in context.Seasons on fixture.SeasonId equals season.SeasonId
                    where season.SeasonId == this.CurrentSeason.SeasonId
                    group team by team.TeamId into t
                    select new
                    {
                        Team = t.Key,
                        Matches = 
                    }
           */


            return Page();
        }
    }
}
