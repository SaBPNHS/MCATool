using System;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;

namespace Sfw.Sabp.Mca.DataAccess.Configuration
{
    public class SqlRetryExecutionStrategy : DbExecutionStrategy
    {
        public SqlRetryExecutionStrategy()
            : base(20, TimeSpan.FromSeconds(5)){}

        protected override bool ShouldRetryOn(Exception exception)
        {
            var retry = false;

            var sqlException = exception as SqlException;

            if (sqlException != null)
            {
                int[] errorsToRetry =
                    {
                        1205,  //Deadlock
                        -2    //Timeout
                    };

                if (sqlException.Errors.Cast<SqlError>().Any(x => errorsToRetry.Contains(x.Number)))
                {
                    retry = true;
                }
                else
                {
                    throw sqlException;
                }
            }
            if (exception is TimeoutException)
            {
                retry = true;
            }
            return retry;
        }
    }
}
