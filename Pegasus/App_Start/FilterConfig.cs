using System.Web;
using System.Web.Mvc;
using Pegasus.Filters;

namespace Pegasus
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            // Custom exception handler
            filters.Add(new ErrorHandlerFilter());
        }
    }
}
