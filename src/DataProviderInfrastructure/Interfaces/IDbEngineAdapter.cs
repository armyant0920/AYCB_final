using System.Data;

namespace DataProviderInfrastructure.Interfaces;

public interface IDbEngineAdapter : IDisposable
{
    IDbConnection CreateConnection(bool autoOpenConnection = true);
}
