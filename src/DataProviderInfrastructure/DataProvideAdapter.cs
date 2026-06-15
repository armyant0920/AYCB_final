using System.Data;
using System.Data.Common;
using System.Text;
using DataProviderInfrastructure.Extensions;
using DataProviderInfrastructure.Interfaces;

namespace DataProviderInfrastructure;

public class DataProvideAdapter : IDisposable
{
    private IDbEngineAdapter _dbEngineAdapter;
    public string ConnectionString { get; protected set; }
    public DbEngineType DbEngineType { get; protected set; }
    private bool _disposed;
    private IDbConnection? _currentConnection;
    private IDbTransaction? _currentTransaction;

    public DataProvideAdapter(string connectionString, DbEngineType dbEngineType)
    {
        ConnectionString = connectionString;
        DbEngineType = dbEngineType;
        _dbEngineAdapter = new DbEngineAdapter(ConnectionString, DbEngineType);
    }

    public IDbConnection GetConnection() => _currentConnection ?? CreateConnection();

    public IDbTransaction GetTransaction()
    {
        var connection = GetConnection();
        if (_currentTransaction == null || _currentTransaction.Connection != connection)
            return BeginTransaction(connection);
        return _currentTransaction;
    }

    public IDbConnection CreateConnection(bool autoOpenConnection = true)
        => _dbEngineAdapter.CreateConnection(autoOpenConnection);

    public IDbTransaction BeginTransaction(IDbConnection connection, IsolationLevel iso = IsolationLevel.Unspecified)
    {
        if (_currentConnection != null && _currentConnection != connection)
        {
            TryClose(_currentConnection);
            _currentConnection = null;
        }

        _currentTransaction?.Dispose();
        _currentTransaction = null;
        _currentConnection = connection;
        _currentTransaction = connection.BeginTransaction(iso);
        return _currentTransaction;
    }

    public IDbCommand CreateCommand(IDbConnection connection, IDbTransaction? transaction = null, ICommandSetting? commandSetting = null)
    {
        if (connection == null) throw new ArgumentNullException(nameof(connection));
        var cmd = connection.CreateCommand();
        cmd.Connection = connection;
        cmd.Transaction = transaction;
        if (commandSetting == null) return cmd;

        cmd.CommandText = commandSetting.SqlCommandText;
        cmd.CommandType = commandSetting.SqlCommandType;
        if (commandSetting.CommandTimeout.HasValue)
            cmd.CommandTimeout = commandSetting.CommandTimeout.Value;

        if (commandSetting.SqlParameters != null)
            foreach (var p in commandSetting.SqlParameters)
                cmd.Parameters.Add(p);

        return cmd;
    }

    protected Exception CreateSqlException(Exception ex, string sql, IEnumerable<DbParameter>? ps = null)
    {
        var sb = new StringBuilder();
        sb.AppendLine("SQL Command:").AppendLine(sql);
        if (ps != null)
            sb.AppendLine("Parameters:").AppendLine(string.Join(", ", ps.Select(p => $"[{p.ParameterName}={p.Value}]")));
        sb.Append(ex);
        return new Exception(sb.ToString(), ex);
    }

    protected T ConvertResult<T>(object? source)
    {
        if (source == DBNull.Value || source == null) return default!;
        return (T)Convert.ChangeType(source, typeof(T));
    }

    private T ExecuteQueryCommand<T>(ICommandSetting commandSetting, Func<IDataReader, T> func) where T : class, new()
    {
        try
        {
            return ExecuteWithConnection((conn, tx) =>
            {
                using var cmd = CreateCommand(conn, null, commandSetting);
                cmd.Transaction = tx;
                using var reader = cmd.ExecuteReader();
                return func(reader);
            });
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            throw CreateSqlException(ex, commandSetting.SqlCommandText, commandSetting.SqlParameters?.OfType<DbParameter>());
        }
    }

    private T ExecuteWithConnection<T>(Func<IDbConnection, IDbTransaction?, T> func)
    {
        using var adapter = new DbEngineAdapter(ConnectionString, DbEngineType);
        using var conn = adapter.CreateConnection();
        return func(conn, null);
    }

    public virtual List<T> Query<T>(string sql, IEnumerable<DbParameter>? sqlParameters = null, CommandType commandType = CommandType.Text) where T : class, new()
    {
        var setting = new CommandSetting(sql, sqlParameters, commandType);
        return ExecuteQueryCommand(setting, reader => reader.ToList<T>());
    }

    public virtual DataTable Query(string sql, IEnumerable<DbParameter>? sqlParameters = null, CommandType commandType = CommandType.Text)
    {
        var setting = new CommandSetting(sql, sqlParameters, commandType);
        return ExecuteQueryCommand<DataTable>(setting, reader =>
        {
            var dt = new DataTable();
            dt.Load(reader);
            return dt;
        });
    }

    public virtual T QueryScalar<T>(string sql, IEnumerable<DbParameter>? sqlParameters = null, CommandType commandType = CommandType.Text)
    {
        var setting = new CommandSetting(sql, sqlParameters, commandType);
        try
        {
            return ExecuteWithConnection((conn, _) =>
            {
                using var cmd = CreateCommand(conn, null, setting);
                return ConvertResult<T>(cmd.ExecuteScalar());
            });
        }
        catch (Exception ex)
        {
            throw CreateSqlException(ex, sql, sqlParameters?.OfType<DbParameter>());
        }
    }

    public int ExecuteNonQueryCommand(ICommandSetting commandSetting)
    {
        if (_currentConnection == null || _currentConnection.State == ConnectionState.Closed)
            throw new InvalidOperationException("No current connection.");
        if (_currentTransaction?.Connection == null)
            throw new InvalidOperationException("No current transaction.");

        using var cmd = CreateCommand(_currentConnection, _currentTransaction, commandSetting);
        try { return cmd.ExecuteNonQuery(); }
        catch (Exception ex) { throw CreateSqlException(ex, commandSetting.SqlCommandText, commandSetting.SqlParameters?.OfType<DbParameter>()); }
    }

    public T ExecuteScalarCommand<T>(ICommandSetting commandSetting, bool appendScopeIdentity = true)
    {
        var sql = commandSetting.SqlCommandText;
        if (appendScopeIdentity && DbEngineType == DbEngineType.SqlServer && !sql.Contains("SCOPE_IDENTITY()", StringComparison.OrdinalIgnoreCase))
            sql += "; SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; ";

        var setting = new CommandSetting(sql, commandSetting.SqlParameters, commandSetting.SqlCommandType);

        if (_currentConnection != null && _currentTransaction?.Connection != null)
        {
            using var cmd = CreateCommand(_currentConnection, _currentTransaction, setting);
            try { return ConvertResult<T>(cmd.ExecuteScalar()); }
            catch (Exception ex) { throw CreateSqlException(ex, sql, commandSetting.SqlParameters?.OfType<DbParameter>()); }
        }

        return ExecuteWithConnection((conn, _) =>
        {
            using var cmd = CreateCommand(conn, null, setting);
            try { return ConvertResult<T>(cmd.ExecuteScalar()); }
            catch (Exception ex) { throw CreateSqlException(ex, sql, commandSetting.SqlParameters?.OfType<DbParameter>()); }
        });
    }

    private static void TryClose(IDbConnection conn)
    {
        try { if (conn.State != ConnectionState.Closed) conn.Close(); conn.Dispose(); }
        catch (ObjectDisposedException) { }
        catch (Exception) { }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _dbEngineAdapter.Dispose();
            _currentTransaction?.Dispose();
            if (_currentConnection != null) TryClose(_currentConnection);
        }
        _disposed = true;
    }

    public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
    ~DataProvideAdapter() { Dispose(false); }
}
