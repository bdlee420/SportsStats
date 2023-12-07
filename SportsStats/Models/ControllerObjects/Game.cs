﻿using System;
using System.Collections.Generic;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Models.ControllerObjects
{
    public class GamesResult
    {
        public List<GameResultBase> Games { get; set; }
        public List<TeamsResult> Teams { get; set; }
    }
    public class GameResultBase
    {
        public int LeagueID { get; set; }
        public int ID { get; set; }
        public int Team1ID { get; set; }
        public string Team1Name { get; set; }
        public int Team1Score { get; set; }
        public int Team2ID { get; set; }
        public string Team2Name { get; set; }
        public int Team2Score { get; set; }
        public DateTime GameDate { get; set; }
    }
    public class GameResult : GameResultBase
    {
        public List<StatTypeResult> QuickStatTypes { get; set; }
        public List<StatTypeResult> StatTypes { get; set; }
        public List<PlayerStatsResult> Team1PlayerStats { get; set; }
        public List<PlayerStatsResult> Team2PlayerStats { get; set; }
        public List<PlayerStatsResult> TotalTeam1Stats { get; set; }
        public List<PlayerStatsResult> TotalTeam2Stats { get; set; }
        public List<TeamsResult> Teams { get; set; }
        public BaseballGameStateResult BaseballGameStateResult { get; set; }
    }
    public class PlayerStatsResult
    {
        public string LeagueName { get; set; }
        public int? PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int PlayerNumber { get; set; }
        public List<StatResult> PlayerStats { get; set; }
        public int? Order { get; set; }
        public bool AtBat { get; set; }
        public SportsList Sport { get; set; }
        public int LeagueID { get; set; }
        public int TeamID { get; set; }
        public bool IsCurrentLeague { get; set; }
        public bool IsActivePlayer { get; set; }
    }
    public class StatResult
    {
        public int StatTypeID { get; set; }
        public string Name { get; set; }
        public bool DefaultShow { get; set; }
        public decimal Value { get; set; }
        public int CurrentValue { get; set; }
        public bool ShowGame { get; set; }
    }
    public class StatTypeResult
    {
        public SportsList Sport { get; set; }
        public int StatTypeID { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public bool DefaultShow { get; set; }
        public int ValueType { get; set; }
        public string QuickButtonText { get; set; }
        public bool IsPositive { get; set; }
        public int QuickButtonOrder { get; set; }
        public bool AutoGenerated { get; set; }
        public bool ShowGame { get; set; }
        public bool ShowTeam { get; set; }
        public bool ShowPlayer { get; set; }
    }
}