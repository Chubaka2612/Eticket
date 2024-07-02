using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ETicket.Api.HealthCheck
{
    public class SqlServerHealthCheck : IHealthCheck
    {
        private readonly string _connectionString;

        public SqlServerHealthCheck(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    await connection.OpenAsync(cancellationToken);
                }
                catch (SqlException)
                {
                    // SQL Server is not reachable or other connection issues
                    return HealthCheckResult.Unhealthy();
                }
                catch (Exception ex)
                {
                    // Other unexpected exceptions
                    return HealthCheckResult.Unhealthy(ex.Message);
                }

                return HealthCheckResult.Healthy();
            }
        }
    }
}
