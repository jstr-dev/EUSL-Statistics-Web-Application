using System;
using System.Collections.Generic;
using EUSL_WebApp.Models;
using EUSL_WebApp.Pages.Team;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace EUSL_WebApp;

public partial class EuslContext : DbContext
{
    public EuslContext()
    {
    }

    public EuslContext(DbContextOptions<EuslContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Fixture> Fixtures { get; set; }

    public virtual DbSet<Nationality> Nationalities { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<PlayerPosition> PlayerPositions { get; set; }

    public virtual DbSet<PlayerToTeam> PlayerToTeams { get; set; }

    public virtual DbSet<Result> Results { get; set; }

    public virtual DbSet<ResultLine> ResultLines { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Team> Teams { get; set; }

    public virtual DbSet<Standing> Standings { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySql("server=localhost;user=root;password=password;database=eusl", ServerVersion.Parse("8.0.27-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Fixture>(entity =>
        {
            entity.HasKey(e => e.FixtureId).HasName("PRIMARY");

            entity.ToTable("fixture");

            entity.HasIndex(e => e.AwayTeamId, "away_team_id");

            entity.HasIndex(e => e.SeasonId, "season_id");

            entity.HasIndex(e => new { e.HomeTeamId, e.AwayTeamId, e.SeasonId, e.Gameweek }, "uc_fixture").IsUnique();

            entity.Property(e => e.FixtureId).HasColumnName("fixture_id");
            entity.Property(e => e.AwayTeamId).HasColumnName("away_team_id");
            entity.Property(e => e.Gameweek).HasColumnName("gameweek");
            entity.Property(e => e.HomeTeamId).HasColumnName("home_team_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");

            entity.HasOne(d => d.AwayTeam).WithMany(p => p.FixtureAwayTeams)
                .HasForeignKey(d => d.AwayTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fixture_ibfk_2");

            entity.HasOne(d => d.HomeTeam).WithMany(p => p.FixtureHomeTeams)
                .HasForeignKey(d => d.HomeTeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fixture_ibfk_1");

            entity.HasOne(d => d.Season).WithMany(p => p.Fixtures)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fixture_ibfk_3");
        });

        modelBuilder.Entity<Nationality>(entity =>
        {
            entity.HasKey(e => e.NationalityId).HasName("PRIMARY");

            entity.ToTable("nationality");

            entity.Property(e => e.NationalityId).HasColumnName("nationality_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PRIMARY");

            entity.ToTable("player");

            entity.HasIndex(e => e.NationalityId, "nationality_id");

            entity.HasIndex(e => e.SlapshotId, "slapshot_id").IsUnique();

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.NationalityId).HasColumnName("nationality_id");
            entity.Property(e => e.SlapshotId).HasColumnName("slapshot_id");

            entity.HasOne(d => d.Nationality).WithMany(p => p.Players)
                .HasForeignKey(d => d.NationalityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("player_ibfk_1");
        });

        modelBuilder.Entity<PlayerPosition>(entity =>
        {
            entity.HasKey(e => e.PositionId).HasName("PRIMARY");

            entity.ToTable("player_position");

            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<PlayerToTeam>(entity =>
        {
            entity.HasKey(e => e.TransferId).HasName("PRIMARY");

            entity.ToTable("player_to_team");

            entity.HasIndex(e => e.PlayerId, "player_id");

            entity.HasIndex(e => e.PositionId, "position_id");

            entity.HasIndex(e => e.SeasonId, "season_id");

            entity.HasIndex(e => e.TeamId, "team_id");

            entity.HasIndex(e => e.TransferedFrom, "transfered_from");

            entity.HasIndex(e => e.TransferedTo, "transfered_to");

            entity.Property(e => e.TransferId).HasColumnName("transfer_id");
            entity.Property(e => e.IsGm).HasColumnName("is_gm");
            entity.Property(e => e.IsInitial)
                .HasDefaultValueSql("'1'")
                .HasColumnName("is_initial");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.TransferDate).HasColumnName("transfer_date");
            entity.Property(e => e.TransferedFrom).HasColumnName("transfered_from");
            entity.Property(e => e.TransferedTo).HasColumnName("transfered_to");

            entity.HasOne(d => d.Player).WithMany(p => p.PlayerToTeams)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("player_to_team_ibfk_1");

            entity.HasOne(d => d.Position).WithMany(p => p.PlayerToTeams)
                .HasForeignKey(d => d.PositionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("player_to_team_ibfk_4");

            entity.HasOne(d => d.Season).WithMany(p => p.PlayerToTeams)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("player_to_team_ibfk_3");

            entity.HasOne(d => d.Team).WithMany(p => p.PlayerToTeams)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("player_to_team_ibfk_2");

            entity.HasOne(d => d.TransferedFromNavigation).WithMany(p => p.InverseTransferedFromNavigation)
                .HasForeignKey(d => d.TransferedFrom)
                .HasConstraintName("player_to_team_ibfk_6");

            entity.HasOne(d => d.TransferedToNavigation).WithMany(p => p.InverseTransferedToNavigation)
                .HasForeignKey(d => d.TransferedTo)
                .HasConstraintName("player_to_team_ibfk_5");
        });

        modelBuilder.Entity<Result>(entity =>
        {
            entity.HasKey(e => e.ResultId).HasName("PRIMARY");

            entity.ToTable("result");

            entity.HasIndex(e => e.FixtureId, "fixture_id").IsUnique();

            entity.HasIndex(e => e.Winner, "winner");

            entity.Property(e => e.ResultId).HasColumnName("result_id");
            entity.Property(e => e.AwayScore).HasColumnName("away_score");
            entity.Property(e => e.DatePlayed).HasColumnName("date_played");
            entity.Property(e => e.FixtureId).HasColumnName("fixture_id");
            entity.Property(e => e.HomeScore).HasColumnName("home_score");
            entity.Property(e => e.IsForfeit)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_forfeit");
            entity.Property(e => e.IsOvertime)
                .HasDefaultValueSql("'0'")
                .HasColumnName("is_overtime");
            entity.Property(e => e.Winner).HasColumnName("winner");

            entity.HasOne(d => d.Fixture).WithOne(p => p.Result)
                .HasForeignKey<Result>(d => d.FixtureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_ibfk_1");

            entity.HasOne(d => d.WinnerNavigation).WithMany(p => p.Results)
                .HasForeignKey(d => d.Winner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_ibfk_2");
        });

        modelBuilder.Entity<ResultLine>(entity =>
        {
            entity.HasKey(e => e.ResultLineId).HasName("PRIMARY");

            entity.ToTable("result_line");

            entity.HasIndex(e => e.ResultId, "result_id");

            entity.HasIndex(e => e.TeamId, "team_id");

            entity.HasIndex(e => new { e.PlayerId, e.ResultId }, "uc_result_line").IsUnique();

            entity.Property(e => e.ResultLineId).HasColumnName("result_line_id");
            entity.Property(e => e.Blocks).HasColumnName("blocks");
            entity.Property(e => e.FaceoffLosses).HasColumnName("faceoff_losses");
            entity.Property(e => e.FaceoffWins).HasColumnName("faceoff_wins");
            entity.Property(e => e.GameWinningGoals).HasColumnName("game_winning_goals");
            entity.Property(e => e.Goals).HasColumnName("goals");
            entity.Property(e => e.GoalsAgainst).HasColumnName("goals_against");
            entity.Property(e => e.GoalsFor).HasColumnName("goals_for");
            entity.Property(e => e.Passes).HasColumnName("passes");
            entity.Property(e => e.PeriodLosses).HasColumnName("period_losses");
            entity.Property(e => e.PeriodTies).HasColumnName("period_ties");
            entity.Property(e => e.PeriodWins).HasColumnName("period_wins");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Possession).HasColumnName("possession");
            entity.Property(e => e.PostHits).HasColumnName("post_hits");
            entity.Property(e => e.PrimaryAssists).HasColumnName("primary_assists");
            entity.Property(e => e.ResultId).HasColumnName("result_id");
            entity.Property(e => e.Saves).HasColumnName("saves");
            entity.Property(e => e.Score).HasColumnName("score");
            entity.Property(e => e.SecondaryAssists).HasColumnName("secondary_assists");
            entity.Property(e => e.Shots).HasColumnName("shots");
            entity.Property(e => e.ShutoutAgainst).HasColumnName("shutout_against");
            entity.Property(e => e.ShutoutFor).HasColumnName("shutout_for");
            entity.Property(e => e.Takeaways).HasColumnName("takeaways");
            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.Turnovers).HasColumnName("turnovers");

            entity.HasOne(d => d.Player).WithMany(p => p.ResultLines)
                .HasForeignKey(d => d.PlayerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_line_ibfk_2");

            entity.HasOne(d => d.Result).WithMany(p => p.ResultLines)
                .HasForeignKey(d => d.ResultId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_line_ibfk_1");

            entity.HasOne(d => d.Team).WithMany(p => p.ResultLines)
                .HasForeignKey(d => d.TeamId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("result_line_ibfk_3");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PRIMARY");

            entity.ToTable("season");

            entity.HasIndex(e => new { e.Num, e.Division }, "uc_season").IsUnique();

            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.Division).HasColumnName("division");
            entity.Property(e => e.Num).HasColumnName("num");
        });

        modelBuilder.Entity<Team>(entity =>
        {
            entity.HasKey(e => e.TeamId).HasName("PRIMARY");

            entity.ToTable("team");

            entity.HasIndex(e => e.Name, "name").IsUnique();

            entity.Property(e => e.TeamId).HasColumnName("team_id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.ShortName)
                .HasMaxLength(4)
                .HasDefaultValueSql("'XXX'")
                .HasColumnName("short_name");
        });

        modelBuilder.Entity<Standing>(entity =>
        { 
            entity.HasNoKey();
            entity.Property(e => e.OtWins).HasColumnName("OT Wins");
            entity.Property(e => e.OtLosses).HasColumnName("OT Losses");
            entity.Property(e => e.GoalsFor).HasColumnName("Goals For");
            entity.Property(e => e.GoalsAgainst).HasColumnName("Goals Against");
            entity.Property(e => e.GoalDifference).HasColumnName("Goal Difference");
        });

        modelBuilder.Entity<Roster>(entity =>
        {
            entity.HasNoKey();
        });  

        OnModelCreatingPartial(modelBuilder);
    }

    public async Task<IEnumerable<Standing>> GetStandings(Season season)
    {
        MySqlParameter seasonId = new MySqlParameter("@seasonid", season.SeasonId);
        var standings = await this.Set<Standing>().FromSqlRaw("CALL GetStandings(@seasonid)", seasonId).ToListAsync();

        return standings;
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
