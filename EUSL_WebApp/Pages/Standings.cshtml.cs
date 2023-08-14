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

        public StandingsModel(EuslContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Standing>> GetStandings(Season season)
        {
            MySqlParameter seasonId = new MySqlParameter("@seasonid", season.SeasonId);
            var standings = await this.context.Set<Standing>().FromSqlRaw(@"
                SELECT `TeamID`, `TeamName`, `Matches`, (`Wins` * 3 + `OT Wins` * 2 + `OT Losses` * 1) AS `Points`, `Wins`, `OT Wins`, `OT Losses`, `Losses`, `Forfeits`, `Goals For`, `Goals Against`, (`Goals For` - `Goals Against`) AS `Goal Difference`
	            FROM (
		            SELECT team.team_id as `TeamID`, 
                    team.name as `TeamName`,
		            COUNT(fixture.fixture_id) as `Matches`, 
		            COUNT(CASE WHEN result.winner = team.team_id AND result.is_overtime = 0 THEN 1 ELSE NULL END) AS `Wins`,
		            COUNT(CASE WHEN result.winner = team.team_id AND result.is_overtime = 1 THEN 1 ELSE NULL END) AS `OT Wins`,
		            COUNT(CASE WHEN result.winner != team.team_id AND result.is_overtime = 1 THEN 1 ELSE NULL END) AS `OT Losses`,
		            COUNT(CASE WHEN coalesce(result.winner,0) != team.team_id AND result.is_overtime = 0 THEN 1 ELSE NULL END) AS `Losses`,
		            COUNT(CASE WHEN coalesce(result.winner,0) != team.team_id AND result.is_forfeit = 1 THEN 1 ELSE NULL END) AS `Forfeits`,
		            SUM(CASE WHEN fixture.home_team_id = team.team_id THEN result.home_score ELSE result.away_score END) AS `Goals For`,
		            SUM(CASE WHEN fixture.home_team_id != team.team_id THEN result.home_score ELSE result.away_score END) AS `Goals Against`
		            FROM team
		            INNER JOIN fixture ON fixture.home_team_id = team.team_id OR fixture.away_team_id = team.team_id
		            INNER JOIN result ON fixture.fixture_id = result.fixture_id
		            INNER JOIN season ON fixture.season_id = season.season_id
		            WHERE season.season_id = @seasonid AND fixture.is_playoff = 0
		            GROUP BY team.team_id
	            ) tbl
	            ORDER BY `Points` DESC, `Matches` ASC, `Forfeits` ASC, (`Wins` + `OT Wins`) DESC, `Wins` DESC, `Goal Difference` DESC, `Goals For` DESC;
            ", seasonId).ToListAsync();
            return standings;
        }

        public async Task<IActionResult> OnGetAsync(int? season)
        {
            this.Seasons = context.Seasons.ToList();

            var current = (season != null) ? await context.Seasons.FirstAsync(sz => sz.SeasonId == season) : await context.Seasons.Where(sz => sz.Division == 1).OrderByDescending(sz => sz.Num).FirstAsync();
            if (current == null)
                return NotFound();

            var standings = await GetStandings(current);
            this.Standings = standings;

            // Filter Settings
            ViewData["CurrentSeason"] = current;
            ViewData["FilterEndpoint"] = "UpdateStandings";
            ViewData["FilterHTMLTarget"] = "standings";

            return Page();
        }

        public async Task<PartialViewResult> OnGetUpdateStandingsAsync(int number, int division)
        {
            var szn = await this.context.Seasons.Where(sz => sz.Division == division && sz.Num == number).FirstAsync();
            ViewData["CurrentSeason"] = szn;

            var standings = await this.GetStandings(szn);

            return new PartialViewResult
            {
                ViewName = "_StandingPartial",
                ViewData = new ViewDataDictionary<IEnumerable<Standing>>(ViewData, standings)
            };
        }
    }
}
