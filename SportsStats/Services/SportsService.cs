using SportsStats.DataProviders;
using SportsStats.Models.ServiceObjects;
using System.Collections.Generic;
using System.Linq;

namespace SportsStats.Services
{
    public class SportsService
    {
        private static readonly SportsService _service = new SportsService();
        public static SportsService GetInstance()
        {
            if (_service == null)
            {
                return new SportsService();
            }
            return _service;
        }

        public List<Sport> GetSports()
        {
            var sports = SportsDataProvider.GetInstance().GetSports();
            return sports.Select(s => new Sport()
            {
                ID = s.ID,
                Name = s.Name
            }).ToList();
        }        
    }
}