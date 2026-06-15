using System.Data;
using DataProviderInfrastructure.Interfaces;
using Microsoft.Data.SqlClient;

namespace DataProviderInfrastructure;

public class DbEngineAdapter : IDbEngineAdapter
{
    private readonly string _connectionString;
    private readonly DbEngineType _engineType;
    private bool _disposed;

    public DbEngineAdapter(string connectionString, DbEngineType engineType)
    {
        _connectionString = connectionString;
        _engineType = engineType;
    }

    public IDbConnection CreateConnection(bool autoOpenConnection = true)
    {
        IDbConnection conn = _engineType switch
        {
            DbEngineType.SqlServer => new SqlConnection(_connectionString),
            _ => throw new NotSupportedException($"DbEngineType {_engineType} is not supported.")
        };

        if (autoOpenConnection)
            conn.Open();

        return conn;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
