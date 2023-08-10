using EUSL_WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EUSL_WebApp.Pages
{
    public class MatchModel : PageModel
    {
        private readonly EuslContext context;
        public Fixture Fixture { get; set; } = default!;
        public Result? Result { get; set; } = null;
        public IEnumerable<ResultLine>? Lines { get; set; } = null;

        public MatchModel(EuslContext context)
        {
            this.context = context;     
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var fixture = await context.Fixtures
                .Include(fix => fix.HomeTeam)
                .Include(fix => fix.AwayTeam)
                .FirstOrDefaultAsync(fix => fix.FixtureId == id);    

            if (fixture == null)
            {
                System.Diagnostics.Debug.WriteLine("Couldn't find Match (ID: " + id + ")", "Query Error");
                return NotFound();
            }

            this.Fixture = fixture;

            // Check whether the fixture has a result object
            var result = await context.Results.FirstOrDefaultAsync(result => result.FixtureId == id);
            if (result != null)
            {
                this.Result = result;

                // Check whether we have result lines, we could not have result lines if the game was a forfeit.
                var query = context.ResultLines
                    .Include(line => line.Player)
                    .Include(line => line.Team)
                    .Where(line => line.ResultId == result.ResultId);

                var resultLines = await query.ToListAsync();
                if (resultLines.Count > 0)
                {
                    this.Lines = resultLines;
                }
            }

            return Page();
        }
    }
}
