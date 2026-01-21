using SportsStats.Models.DTOObjects;
using SportsStats.Models.ServiceObjects;
using System.Linq;
using System.Web;
using static SportsStats.Helpers.Enums;

namespace SportsStats.Helpers
{
    public class UserHelper
    {
        public static User CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session["CurrentUser"] != null)
                    return (User)HttpContext.Current.Session["CurrentUser"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["CurrentUser"] = value;
            }
        }

        public static bool HasAdminPermissions(int? teamID = null, int? leagueID = null)
        {
            if (CurrentUser == null)
                return false;

            if (CurrentUser.Role == Roles.Admin)
                return true;

            return false;
        }

        public static bool HasUpdatePermissions(int? teamID = null, int? leagueID = null)
        {
            if (CurrentUser == null)
                return false;

            if (CurrentUser.Role == Roles.Admin)
                return true;

            if (leagueID.HasValue && CurrentUser.AdminLeagueIDs.Contains(leagueID.Value))
                return true;

            return false;
        }

        public static bool HasGetPermissions(int? teamID = null, int? leagueID = null, int? playerID = null, int? gameID = null, int? sportID = null)
        {
            if (CurrentUser == null)
                return false;

            if (CurrentUser.Role == Roles.Admin)
                return true;

            if (CurrentUser.Role != Roles.User)
                return false;

            if (teamID.HasValue && CurrentUser.Teams != null && CurrentUser.Teams.Exists(t => t == teamID))
                return true;

            if (leagueID.HasValue && CurrentUser.Leagues != null && CurrentUser.Leagues.Exists(t => t == leagueID))
                return true;

            if (playerID.HasValue && CurrentUser.Players != null && CurrentUser.Players.Exists(t => t == playerID))
                return true;

            if (sportID.HasValue && CurrentUser.Sports != null && CurrentUser.Sports.Exists(t => t == sportID))
                return true;

            if (gameID.HasValue)
                return true;

            return false;
        }

        public static bool HasGetPermissions(Game game)
        {
            if (CurrentUser.Role == Roles.Admin)
                return true;

            if (CurrentUser.AdminLeagueIDs.Contains(game.LeagueID))
                return true;

            if (CurrentUser.Role == Roles.User && (CurrentUser.Teams.Exists(t => t == game.Team1ID) || CurrentUser.Teams.Exists(t => t == game.Team2ID)))
                return true;

            return false;
        }
    }
}