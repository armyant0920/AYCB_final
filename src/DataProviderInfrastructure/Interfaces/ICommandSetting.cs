using System.Data;
using System.Data.Common;

namespace DataProviderInfrastructure.Interfaces;

public interface ICommandSetting
{
    string SqlCommandText { get; }
    IEnumerable<DbParameter>? SqlParameters { get; }
    CommandType SqlCommandType { get; }
    int? CommandTimeout { get; }
}
