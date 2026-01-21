using System;
using System.Collections.Generic;
using System.Web.Http;
using SportsStats.Models.ServiceObjects;
using SportsStats.Services;

namespace SportsStats.Controllers
{
    public class SportsController : ApiController
    {
        [HttpGet]
        public List<Sport> GetSports()
        {
            try
            {
                return SportsService.GetInstance().GetSports();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }        
    }
}