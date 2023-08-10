using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using EUSL_WebApp;
using EUSL_WebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace EUSL_WebApp.Pages.Team
{
    public class TeamModel : PageModel
    {
        private readonly EuslContext context;
        public Models.Team Team { get; set; } = default!;
        public IEnumerable<Player> Roster { get; set; } = default!;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public Dictionary<Player, int[]> Statistics { get; set; } = new Dictionary<Player, int[]>();

        public TeamModel(EuslContext context)
        {
            this.context = context;
        }

        public async Task<IActionResult> OnGet([FromRoute] int id, int? seasonid)
        {
            var team = await context.Teams.FirstOrDefaultAsync(team => team.TeamId == id);
            if (team == null)
                return NotFound();
            this.Team = team;

            this.Seasons = await context.PlayerToTeams
                .Include(ptt => ptt.Season)
                .Where(ptt => ptt.TeamId == id)
                .Select(ptt => ptt.Season)
                .Distinct()
                .OrderByDescending(season => season.Num)
                .ToListAsync();

            Season? season;
            if (seasonid != null)
                season = this.Seasons.Where(season => season.SeasonId == seasonid).FirstOrDefault();
            else
                season = this.Seasons.FirstOrDefault();

            if (season == null)
                return NotFound("No seasons were found or seasonid was incorrect.");

            this.Roster = await context.PlayerToTeams
                .Include(ptt => ptt.Player)
                .Where(ptt => ptt.SeasonId == season.SeasonId && ptt.TeamId == id)
                .Select(ptt => ptt.Player)
                .ToListAsync();

            // We can some stats to show
            var playerStats = from fixture in context.Fixtures
                              join result in context.Results on fixture.FixtureId equals result.FixtureId
                              join resultline in context.ResultLines on new { id = result.ResultId, player = id } equals new { id = resultline.ResultId, player = resultline.TeamId }
                              group resultline by resultline.Player into player
                              select new
                              {
                                  Player = player.Key,
                                  Goals = player.Sum(line => line.Goals),
                                  Assists = player.Sum(line => line.PrimaryAssists) + player.Sum(line => line.SecondaryAssists),
                                  Saves = player.Sum(line => line.Saves)
                              };
                              
            foreach (var stat in playerStats)
            {
                Statistics.Add(stat.Player, new int[] { stat.Goals, stat.Assists, stat.Saves });
            }

            return Page();
        }
        
    }
}
