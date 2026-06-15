using DataProviderInfrastructure;
using Microsoft.Extensions.Configuration;

namespace CorporateSite.Infrastructure.Data;

public class DataProviderFactory
{
    private readonly string _connectionString;
    private readonly DbEngineType _engineType;

    public DataProviderFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");

        var engineStr = configuration["Database:Engine"] ?? "SqlServer";
        _engineType = Enum.TryParse<DbEngineType>(engineStr, true, out var e) ? e : DbEngineType.SqlServer;
    }

    public DataProvideAdapter Create() => new(_connectionString, _engineType);
}
