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
using MySqlConnector;

namespace EUSL_WebApp.Pages.Team
{
    public class Roster
    {
        public Models.Player Player { get; set; } = default!;
        public int Goals { get; set; } = default!;
        public int Assists { get; set; } = default!;
        public int Saves { get; set; } = default!;
        public int Shots { get; set; } = default!;
        public int Matches { get; set; } = default!;
        public int Periods { get; set; } = default!;    
        public int Wins { get; set; } = default!;
        public int Losses { get; set; } = default!;
        public int GetPoints()
        {
            return Goals + Assists;
        }
    }

    public class TeamModel : PageModel
    {
        private readonly EuslContext context;
        public Models.Team Team { get; set; } = default!;
        public IEnumerable<Season> Seasons { get; set; } = default!;
        public Season Current { get; set; } = default!;
        public List<Roster> Roster { get; set; } = default!;

        public TeamModel(EuslContext context)
        {
            this.context = context;
        }

        /* 
         * I can't seem to get this to work.
         * public async Task<List<Roster>> GetRoster(int season, int team)
         {
             var subquery = from line in context.ResultLines
                            join result in context.Results on line.ResultId equals result.ResultId
                            join fixture in context.Fixtures on result.FixtureId equals fixture.FixtureId
                            where fixture.SeasonId == season
                            let id = result.Winner
                            group line by line.PlayerId into player
                            select new
                            {
                                Player = player.Key,
                                Goals = player.Sum(line => line.Goals),
                                Assists = player.Sum(line => line.PrimaryAssists + line.SecondaryAssists),
                                Saves = player.Sum(line => line.Saves),
                                Shots = player.Sum(line => line.Shots),
                                Matches = player.Count(),
                                Periods = player.Sum(line => line.PeriodLosses + line.PeriodWins + line.PeriodTies),
                                Wins = player.Count()
                            };

             this.Roster = await (from ptt in context.PlayerToTeams
                                  where (ptt.SeasonId == season && ptt.TeamId == team)
                                  join s in subquery on ptt.PlayerId equals s.Player into stats
                                  from stat in stats.DefaultIfEmpty()
                                  select new Roster()
                                  {
                                      Player = ptt.Player,
                                      Goals = (int?) stat.Goals ?? 0,
                                      Assists = (int?) stat.Assists ?? 0,
                                      Saves = (int?) stat.Saves ?? 0,
                                      Shots = (int?) stat.Shots ?? 0,
                                      Matches = (int?) stat.Matches ?? 0,
                                      Periods = (int?) stat.Periods ?? 0,
                                      Wins = (int?) stat.Wins ?? 0,
                                  }).ToListAsync();

             return this.Roster;
         }*/

        public async Task<List<Roster>> GetRoster(int season, int team, int playoff)
        {
            MySqlParameter seasonId = new MySqlParameter("@seasonid", season);
            MySqlParameter teamId = new MySqlParameter("@teamid", team);
            MySqlParameter playoffId = new MySqlParameter("@playoff", playoff);
            var roster = await context.Set<Roster>().FromSqlRaw(@"
                SELECT 
                    player.player_id as PlayerId,
                    COALESCE(goals, 0) AS Goals,
                    COALESCE(assists, 0) AS Assists,
                    COALESCE(shots, 0) AS Shots,
                    COALESCE(saves, 0) AS Saves,
                    COALESCE(matches, 0) AS Matches,
                    COALESCE(wins, 0) AS Wins,
                    COALESCE(losses, 0) AS Losses,
                    COALESCE(periods, 0) AS Periods
                FROM
                    player_to_team
                        LEFT JOIN
                    (SELECT 
                        player_id, 
                        season_id, 
                        SUM(goals) AS goals,
                        SUM(primary_assists + secondary_assists) AS assists,
                        SUM(shots) AS shots,
                        SUM(saves) AS saves,
                        COUNT(player_id) AS matches,
                        COUNT(CASE WHEN winner = team_id THEN 1 ELSE NULL END) AS wins,
		                COUNT(CASE WHEN winner != team_id THEN 1 ELSE NULL END) AS losses,
                        SUM(period_wins + period_losses + period_ties) as periods
                    FROM
                        result_line
                    INNER JOIN result ON result_line.result_id = result.result_id
                    INNER JOIN fixture ON fixture.fixture_id = result.fixture_id
                    WHERE fixture.is_playoff = @playoff
                    GROUP BY player_id, season_id) AS stats ON player_to_team.player_id = stats.player_id
                        AND stats.season_id = player_to_team.season_id
	                INNER JOIN player ON player.player_id = player_to_team.player_id
                WHERE
                    player_to_team.season_id = @seasonid
                        AND player_to_team.team_id = @teamid
            ", seasonId, teamId, playoffId)
                .Include(roster => roster.Player)
                .ToListAsync();

            return roster;
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

            var season = seasonid != null ? this.Seasons.Where(season => season.SeasonId == seasonid).FirstOrDefault() : this.Seasons.FirstOrDefault();
            if (season == null)
                return NotFound("No season found.");

            this.Current = season;
            this.Roster = await GetRoster(this.Current.SeasonId, this.Team.TeamId, 0);

            return Page();
        }

        public async Task<PartialViewResult> OnGetUpdateRosterAsync(int season, int team, int playoff)
        {
            this.Roster = await GetRoster(season, team, playoff);

            return new PartialViewResult
            {
                ViewName = "_RosterPartial",
                ViewData = new ViewDataDictionary<List<Roster>>(ViewData, this.Roster)
            };
        }
    }
}
