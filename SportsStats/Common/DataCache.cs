using System.Collections.Generic;
using System.Data;

namespace SportsStats.Common
{
    public class DataCache
    {
        public DataCache()
        {
            GameResult = new Dictionary<CacheKey, DataTable>();
            StatsResult = new Dictionary<CacheKey, DataTable>();
            StatesResult = new Dictionary<CacheKey, DataTable>();
            PlayersResult = new Dictionary<CacheKey, DataTable>();
            TeamsResult = new Dictionary<CacheKey, DataTable>();
            BaseballStateResult = new Dictionary<CacheKey, DataTable>();
            StatTypesResult = new Dictionary<CacheKey, DataTable>();
        }

        public Dictionary<CacheKey, DataTable> GameResult { get; set; }
        public Dictionary<CacheKey, DataTable> StatsResult { get; set; }
        public Dictionary<CacheKey, DataTable> StatesResult { get; set; }
        public Dictionary<CacheKey, DataTable> PlayersResult { get; set; }
        public Dictionary<CacheKey, DataTable> TeamsResult { get; set; }
        public Dictionary<CacheKey, DataTable> StatTypesResult { get; set; }
        public Dictionary<CacheKey, DataTable> BaseballStateResult { get; set; }
    }

    public class CacheKey
    {
        public CacheKey(int? gameID, int? teamID, int? playerID, int? leagueID, int? sportID = null)
        {
            GameID = gameID;
            TeamID = teamID;
            PlayerID = playerID;
            SportID = sportID;
        }

        public int? GameID { get; set; }
        public int? TeamID { get; set; }
        public int? PlayerID { get; set; }
        public int? LeagueID { get; set; }
        public int? SportID { get; set; }

        public bool Equals(CacheKey x, CacheKey y)
        {
            return x.GameID == y.GameID && x.TeamID == y.TeamID && x.PlayerID == y.PlayerID && x.LeagueID == y.LeagueID && x.SportID == y.SportID;
    }

        public int GetHashCode(CacheKey x)
        {
            return x.GameID.GetHashCode() + x.TeamID.GetHashCode() + x.PlayerID.GetHashCode() + x.LeagueID.GetHashCode() + x.SportID.GetHashCode();
        }
    }
}