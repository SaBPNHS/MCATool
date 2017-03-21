using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Sfw.Sabp.Mca.DataAccess.Configuration;

namespace Sfw.Sabp.Mca.DataAccess.Ef.Configuration
{
    public class McaConfiguration : DbConfiguration
    {
        public McaConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlRetryExecutionStrategy());
            SetDefaultConnectionFactory(new SqlConnectionFactory("Mca"));
        }
    } 
}
