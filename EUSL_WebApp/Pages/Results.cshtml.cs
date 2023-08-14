using EUSL_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace EUSL_WebApp.Pages
{
    public class ResultObject
    {
        public Models.Team Home = default!;
        public Models.Team Away = default!;
        public int HomeScore;
        public int AwayScore;
        public int Gameweek;
        public bool IsFF;
        public bool IsOT;
        public bool IsPlayoff;
        public int Fixture;
    }

    public class ResultsModel : PageModel
    {
        private readonly EuslContext context;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public IEnumerable<ResultObject> Results { get; set; } = default!;

        public ResultsModel(EuslContext context)
        {
            this.context = context;
        }

        private async Task<IEnumerable<ResultObject>> GetResults(int number, int division)
        {
            var results = from result in context.Results
                          join fixture in context.Fixtures
                            on result.FixtureId equals fixture.FixtureId
                          where fixture.Season.Num == number && fixture.Season.Division == division
                          select new ResultObject
                          {
                              Home = fixture.HomeTeam,
                              Away = fixture.AwayTeam,
                              HomeScore = result.HomeScore,
                              AwayScore = result.AwayScore,
                              Gameweek = fixture.Gameweek,
                              IsFF = result.IsForfeit == 1,
                              IsOT = result.IsOvertime == 1,
                              IsPlayoff = fixture.IsPlayoff == 1,
                              Fixture = fixture.FixtureId
                          };

            return await results.ToListAsync();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            this.Seasons = context.Seasons.ToList();

            // Get the current pro season
            var current = await context.Seasons.Where(season => season.Division == 1).OrderByDescending(season => season.Num).FirstAsync();
            if (current == null)
                return NotFound();

            this.Results = await GetResults(current.Num, current.Division);


            // Filter Settings
            ViewData["CurrentSeason"] = current;
            ViewData["FilterEndpoint"] = "UpdateResults";
            ViewData["FilterHTMLTarget"] = "results";

            return Page();
        }

        public async Task<PartialViewResult> OnGetUpdateResultsAsync(int number, int division)
        {
            this.Results = await GetResults(number, division);

            return new PartialViewResult
            {
                ViewName = "_ResultsPartial",
                ViewData = new ViewDataDictionary<IEnumerable<ResultObject>>(ViewData, this.Results)
            };
        }
    }
}
