using SportsStats.Models.DTOObjects;
using System.Collections.Generic;
using System.Data;

namespace SportsStats.DataProviders
{
    public class SportsDataProvider : DataAccess
    {
        private static readonly SportsDataProvider _dataProvider = new SportsDataProvider();
        public static SportsDataProvider GetInstance()
        {
            if (_dataProvider == null)
            {
                return new SportsDataProvider();
            }
            return _dataProvider;
        }       
        public List<DTOSports> GetSports()
        {
            var sports = new List<DTOSports>();
            var result = SQLGetDataTable("GetSports");
            foreach (DataRow dr in result.Rows)
            {
                sports.Add(new DTOSports()
                {
                    ID = (int)dr["ID"],
                    Name = dr["Name"].ToString()
                });
            }
            return sports;
        }
    }
}