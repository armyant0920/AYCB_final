using System.Data;
using System.Data.Common;
using DataProviderInfrastructure.Interfaces;

namespace DataProviderInfrastructure;

public class CommandSetting : ICommandSetting
{
    public string SqlCommandText { get; }
    public IEnumerable<DbParameter>? SqlParameters { get; }
    public CommandType SqlCommandType { get; }
    public int? CommandTimeout { get; private set; }

    public CommandSetting(string sql, IEnumerable<DbParameter>? sqlParameters = null, CommandType commandType = CommandType.Text)
    {
        SqlCommandText = sql;
        SqlParameters = sqlParameters;
        SqlCommandType = commandType;
    }

    public void SetCommandTimeout(int? timeout) => CommandTimeout = timeout;
}
