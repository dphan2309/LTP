using System.Configuration;
using System.Data.SqlClient;

namespace LTP.DataAccess
{
    public class BaseDao
    {
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(ConfigurationManager.ConnectionStrings["LTPConnectionString"].ConnectionString);
        }
    }
}