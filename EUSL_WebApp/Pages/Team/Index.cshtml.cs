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
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EUSL_WebApp.Pages.Team
{
    public class Roster
    {
        public Player Player { get; set; } = default!;
        public int? Goals { get; set; } = 0;
        public int? Assists { get; set; } = 0;
        public int? Saves { get; set; } = 0;
    }

    public class TeamModel : PageModel
    {
        private readonly EuslContext context;
        public Models.Team Team { get; set; } = default!;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public Season? Current = default!;
        public List<Roster> Roster { get; set; } = default!;

        public TeamModel(EuslContext context)
        {
            this.context = context;
        }

        public async Task<List<Roster>> GetRoster(int season, int team)
        {
            var subquery = from line in context.ResultLines
                           join result in context.Results on line.ResultId equals result.ResultId
                           join fixture in context.Fixtures on result.FixtureId equals fixture.FixtureId
                           where fixture.SeasonId == season
                           group line by line.PlayerId into player
                           select new
                           {
                               Player = player.Key,
                               Goals = player.Sum(line => line.Goals),
                               Assists = player.Sum(line => line.PrimaryAssists + line.SecondaryAssists),
                               Saves = player.Sum(line => line.Saves)
                           };

            this.Roster = await (from ptt in context.PlayerToTeams
                                 where (ptt.SeasonId == season && ptt.TeamId == team)
                                 join s in subquery on ptt.PlayerId equals s.Player into stats
                                 from stat in stats.DefaultIfEmpty()
                                 select new Roster()
                                 {
                                     Player = ptt.Player,
                                     Goals = stat.Goals,
                                     Assists = stat.Assists,
                                     Saves = stat.Saves
                                 }).ToListAsync();

            return this.Roster;
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

            if (seasonid != null)
                this.Current = this.Seasons.Where(season => season.SeasonId == seasonid).FirstOrDefault();
            else
                this.Current = this.Seasons.FirstOrDefault();

            if (this.Current == null)
                return NotFound("No seasons were found or seasonid was incorrect.");

            this.Roster = await GetRoster(this.Current.SeasonId, this.Team.TeamId);

            return Page();
        }

        public async Task<PartialViewResult> OnGetUpdateRosterAsync(int season, int team)
        {
            System.Diagnostics.Debug.WriteLine("I was called~!");
            this.Roster = await GetRoster(season, team);

            return new PartialViewResult
            {
                ViewName = "_RosterPartial",
                ViewData = new ViewDataDictionary<List<Roster>>(ViewData, this.Roster)
            };
        }
    }
}
