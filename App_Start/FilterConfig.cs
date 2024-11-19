using System.Web;
using System.Web.Mvc;
using WebsiteBanDoAnVaThucUong.Filters;

namespace WebsiteBanDoAnVaThucUong
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new StoreSelectorFilter()); 
        }
    }
}
