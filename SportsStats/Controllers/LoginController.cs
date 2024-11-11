using SportsStats.Helpers;
using SportsStats.Models.ControllerObjects;
using SportsStats.Services;
using System;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Security;

namespace SportsStats.Controllers
{
    public class LoginController : ApiController
    {
        private const string _cookieName = "username";
        [HttpPost]
        public bool LoginUser([FromBody]UserResult user)
        {
            var userResult = UserService.GetInstance().GetUser(ConvertObjects.ConvertType(user), true);
            bool validLogin = false;
            if (userResult.Role > 0)
            {
                validLogin = true;
                if (user.RememberMe)
                {
                    CreateUserCookie(user.UserName);
                }
            }
            return validLogin;
        }

        [HttpGet]
        public UserResult GetUser(int? leagueID = null)
        {
            var loggedInUser = UserService.GetInstance().GetUser();
            if (loggedInUser == null)
            {
                var cookie = HttpContext.Current.Request.Cookies[_cookieName];
                if (cookie != null && !String.IsNullOrEmpty(cookie.Value))
                {
                    try
                    {
                        string username = Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(cookie.Value)));
                        UserResult user = new UserResult()
                        {
                            UserName = username
                        };
                        loggedInUser = UserService.GetInstance().GetUser(ConvertObjects.ConvertType(user), false);                        
                    }
                    catch
                    {
                        return null;
                    }
                }                                
            }
            if (loggedInUser != null)
            {
                loggedInUser.HasOneTeam = loggedInUser.Teams.Count == 1;
                loggedInUser.HasOneLeague = loggedInUser.Leagues.Count == 1;
            }
            return ConvertObjects.ConvertType2(loggedInUser);
        }

        [HttpGet]
        public void Logout()
        {
            UserHelper.CurrentUser = null;
        }

        private void CreateUserCookie(string userName)
        {
            var encryptedString = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(userName)));
            HttpCookie myCookie = new HttpCookie(_cookieName, encryptedString);
            myCookie.Expires = DateTime.Now.AddDays(30);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }        
    }
}