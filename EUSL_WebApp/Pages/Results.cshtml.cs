using EUSL_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EUSL_WebApp.Pages
{
    public class ResultsModel : PageModel
    {
        private readonly EuslContext context;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public Season CurrentSeason { get; set; } = default!;
        public IEnumerable<ResultObject> Results { get; set; } = default!;

        public class ResultObject
        {
            public Models.Team Home = default!;
            public Models.Team Away = default!;
            public int HomeScore;
            public int AwayScore;
            public int Gameweek;
            public bool IsFF;
            public bool IsOT;
            public int Fixture;
        }

        public ResultsModel(EuslContext context)
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
            }
            else
            {
                return NotFound();
            }

            // I can try and use the other syntax to do this better/faster later and remove ResultObject (use Fixture instead)
            var results = from result in context.Results
                          join fixture in context.Fixtures
                            on result.FixtureId equals fixture.FixtureId
                          where fixture.Season == CurrentSeason
                          select new ResultObject
                          {
                              Home = fixture.HomeTeam,
                              Away = fixture.AwayTeam,
                              HomeScore = result.HomeScore,
                              AwayScore = result.AwayScore,
                              Gameweek = fixture.Gameweek,
                              IsFF = result.IsForfeit == 1,
                              IsOT = result.IsOvertime == 1,
                              Fixture = fixture.FixtureId
                          };

            this.Results = await results.ToListAsync();

            return Page();
        }
    }
}
