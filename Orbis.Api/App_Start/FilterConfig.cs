using System.Web;
using System.Web.Mvc;
using Orbis.Api.Controllers;

namespace Orbis.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}