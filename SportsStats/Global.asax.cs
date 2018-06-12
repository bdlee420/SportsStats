using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Routing;
using System.Web.SessionState;

namespace SportsStats
{
	public class Global : System.Web.HttpApplication
    {
        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api");
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            // Controller Only
            // To handle routes like `/api/VTRouting`
            RouteTable.Routes.MapHttpRoute(
                name: "ControllerOnly",
                routeTemplate: "api/{controller}"
            );

            RouteTable.Routes.MapHttpRoute(
                name: "ControllerAndId",
                routeTemplate: "api/{controller}/{id}",
                defaults: null,
                constraints: new { id = @"^\d+$" } // Only integers 
            );

            RouteTable.Routes.MapHttpRoute(
                 "ControllerAndAction",
                 "api/{controller}/{action}"
             );

            RouteTable.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get.ToString()) });
            RouteTable.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post.ToString()) });

			GlobalConfiguration.Configuration.Filters.Add(new LogExceptionFilterAttribute());
		}        
    }

	public class LogExceptionFilterAttribute : ExceptionFilterAttribute
	{
		private static Logger logger = LogManager.GetCurrentClassLogger();

		public override void OnException(HttpActionExecutedContext context)
		{
			logger.Error(context.Exception.Message + "..." + context.Exception.StackTrace);
		}
	}
}