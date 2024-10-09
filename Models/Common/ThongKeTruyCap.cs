using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Dapper;

namespace WebsiteBanDoAnVaThucUong.Models.Common
{
    public class ThongKeTruyCap
    {
        public static string strConnect = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();

        public static ThongKeViewModel ThongKe()
        {
            using (var connect = new SqlConnection(strConnect))
            {
                // using Dapper
                var item = connect.QueryFirstOrDefault<ThongKeViewModel>("sp_ThongKe", commandType: CommandType.StoredProcedure);
                return item;
            }
        }
    }
}