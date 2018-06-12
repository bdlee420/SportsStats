using SportsStats.DataProviders;
using SportsStats.Helpers;
using SportsStats.Models.ServiceObjects;
using System;
using System.Configuration;

namespace SportsStats.Services
{
    public class UserService
    {
        private static readonly UserService _service = new UserService();
        private static bool ForceAdmin = ConfigurationManager.AppSettings["ForceAdmin"] == "true";

        public static UserService GetInstance()
        {
            if (_service == null)
            {
                return new UserService();
            }
            return _service;
        }

        public User GetUser(User user, bool checkPassword)
        {
            if (checkPassword && String.IsNullOrEmpty(user.Password))
                return null;

            var result = UserDataProvider.GetInstance().GetUser(ConvertObjects.ConvertType(user));
            UserHelper.CurrentUser = ConvertObjects.ConvertType(result);
            return UserHelper.CurrentUser;
        }

        public User GetUser()
        {
            if (ForceAdmin)
            {
                UserHelper.CurrentUser = new User() { UserName = "bdlee420", Role = Enums.Roles.Admin };
            }
            return UserHelper.CurrentUser;
        }
    }
}