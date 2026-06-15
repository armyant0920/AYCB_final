using System.Data;
using System.Reflection;

namespace DataProviderInfrastructure.Extensions;

public static class DataReaderExtensions
{
    public static List<T> ToList<T>(this IDataReader reader) where T : class, new()
    {
        var result = new List<T>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var columns = Enumerable.Range(0, reader.FieldCount)
            .Select(i => reader.GetName(i))
            .ToList();

        while (reader.Read())
        {
            var item = new T();
            foreach (var col in columns)
            {
                if (!props.TryGetValue(col, out var prop)) continue;
                var val = reader[col];
                if (val == DBNull.Value || val == null) continue;
                try
                {
                    prop.SetValue(item, Convert.ChangeType(val, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                }
                catch { /* column type mismatch — skip */ }
            }
            result.Add(item);
        }
        return result;
    }
}
