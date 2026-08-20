using Microsoft.EntityFrameworkCore;
using PayrollManagementSystem.Application.Common.Interfaces;
using PayrollManagementSystem.Application.Features.SystemManagement.DTOs;
using PayrollManagementSystem.Application.Wrappers;
using System.Data;
using System.Data.Common;

namespace PayrollManagementSystem.Infrastructure.Repositories
{
    public class SystemLogRepository : ISystemLogRepository
    {
        private readonly IApplicationDbContext _context;

        private static readonly Dictionary<int, string> LevelMap = new()
        {
            [0] = "Verbose",
            [1] = "Debug",
            [2] = "Information",
            [3] = "Warning",
            [4] = "Error",
            [5] = "Fatal",
        };

        private static readonly Dictionary<string, int> LevelToInt = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Verbose"] = 0,
            ["Debug"] = 1,
            ["Information"] = 2,
            ["Warning"] = 3,
            ["Error"] = 4,
            ["Fatal"] = 5,
        };

        public SystemLogRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResponse<List<SystemLogDto>>> GetLogsAsync(
            string? level, DateTime? fromDate, DateTime? toDate, string? keyword,
            string? sortBy, string? sortDirection,
            int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync(cancellationToken);

            var tableName = await FindTableNameAsync(connection, cancellationToken);
            if (tableName == null)
            {
                return new PagedResponse<List<SystemLogDto>>(
                    new List<SystemLogDto>(), pageNumber, pageSize, 0,
                    "Bảng SystemLogs chưa tồn tại.");
            }

            var qt = $"\"{tableName}\"";
            var schema = await DiscoverSchemaAsync(connection, qt, cancellationToken);

            var tsCol = FindCol(schema.Keys, "raise_date", "timestamp", "time_stamp", "raised_at", "logged_at", "created_at")
                          ?? throw new InvalidOperationException($"Không tìm thấy cột timestamp. Cột: {string.Join(", ", schema.Keys)}");
            var lvlCol = FindCol(schema.Keys, "level")
                          ?? throw new InvalidOperationException($"Không tìm thấy cột level. Cột: {string.Join(", ", schema.Keys)}");
            var msgCol = FindCol(schema.Keys, "message", "rendered_message")
                          ?? throw new InvalidOperationException($"Không tìm thấy cột message. Cột: {string.Join(", ", schema.Keys)}");
            var excCol = FindCol(schema.Keys, "exception");
            var propCol = FindCol(schema.Keys, "properties", "log_event");

            var levelIsInt = schema.TryGetValue(lvlCol, out var lvlType) &&
                             (lvlType.Contains("int") || lvlType.Contains("Int"));

            var where = new List<string>();
            var ps = new List<(string name, object value)>();

            if (!string.IsNullOrWhiteSpace(level))
            {
                if (levelIsInt && LevelToInt.TryGetValue(level, out var lvlInt))
                {
                    where.Add($"{qt}.\"{lvlCol}\" = @level");
                    ps.Add(("@level", lvlInt));
                }
                else if (!levelIsInt)
                {
                    where.Add($"{qt}.\"{lvlCol}\" = @level");
                    ps.Add(("@level", level));
                }
            }
            if (fromDate.HasValue)
            {
                where.Add($"{qt}.\"{tsCol}\" >= @fromDate");
                ps.Add(("@fromDate", DateTime.SpecifyKind(fromDate.Value, DateTimeKind.Utc)));
            }
            if (toDate.HasValue)
            {
                where.Add($"{qt}.\"{tsCol}\" < @toDate");
                ps.Add(("@toDate", DateTime.SpecifyKind(toDate.Value.AddDays(1), DateTimeKind.Utc)));
            }
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                where.Add($"{qt}.\"{msgCol}\" ILIKE @keyword");
                ps.Add(("@keyword", $"%{keyword}%"));
            }

            var whereStr = where.Count > 0 ? "WHERE " + string.Join(" AND ", where) : "";

            long totalRecords = 0;
            await using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = $"SELECT COUNT(*) FROM {qt} {whereStr}";
                ApplyParams(cmd, ps);
                totalRecords = Convert.ToInt64(await cmd.ExecuteScalarAsync(cancellationToken) ?? 0L);
            }

            var orderByCol = tsCol;
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                if (sortBy.Equals("level", StringComparison.OrdinalIgnoreCase)) orderByCol = lvlCol;
                else if (sortBy.Equals("message", StringComparison.OrdinalIgnoreCase)) orderByCol = msgCol;
                else if (sortBy.Equals("raiseDate", StringComparison.OrdinalIgnoreCase)) orderByCol = tsCol;
            }
            var direction = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

            var logs = new List<SystemLogDto>();
            await using (var cmd = connection.CreateCommand())
            {
                var excSql = excCol != null ? $"{qt}.\"{excCol}\"" : "NULL";
                var propSql = propCol != null ? $"{qt}.\"{propCol}\"" : "NULL";

                cmd.CommandText = $@"
                    SELECT
                        ROW_NUMBER() OVER (ORDER BY {qt}.""{orderByCol}"" {direction}) AS __id,
                        {qt}.""{tsCol}"",
                        {qt}.""{lvlCol}"",
                        {qt}.""{msgCol}"",
                        {excSql},
                        {propSql}
                    FROM {qt}
                    {whereStr}
                    ORDER BY {qt}.""{orderByCol}"" {direction}
                    LIMIT @pageSize OFFSET @offset";

                ApplyParams(cmd, ps);
                AddParam(cmd, "@pageSize", pageSize);
                AddParam(cmd, "@offset", (pageNumber - 1) * pageSize);

                await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                while (await reader.ReadAsync(cancellationToken))
                {
                    var rawLevel = reader.GetValue(2);
                    var levelStr = rawLevel switch
                    {
                        int i => LevelMap.GetValueOrDefault(i, i.ToString()),
                        long l => LevelMap.GetValueOrDefault((int)l, l.ToString()),
                        string s => s,
                        _ => rawLevel?.ToString() ?? "Information"
                    };

                    logs.Add(new SystemLogDto
                    {
                        Id = reader.GetInt64(0),
                        RaiseDate = reader.GetDateTime(1),
                        Level = levelStr,
                        Message = reader.IsDBNull(3) ? null : reader.GetValue(3)?.ToString(),
                        Exception = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString(),
                        Properties = reader.IsDBNull(5) ? null : reader.GetValue(5)?.ToString(),
                    });
                }
            }

            return new PagedResponse<List<SystemLogDto>>(logs, pageNumber, pageSize, (int)totalRecords);
        }

        private static async Task<string?> FindTableNameAsync(DbConnection conn, CancellationToken ct)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT tablename FROM pg_tables WHERE LOWER(tablename) = 'systemlogs' AND schemaname = 'public' LIMIT 1";
            return (await cmd.ExecuteScalarAsync(ct)) as string;
        }

        private static async Task<Dictionary<string, string>> DiscoverSchemaAsync(DbConnection conn, string quotedTable, CancellationToken ct)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {quotedTable} LIMIT 0";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            for (int i = 0; i < reader.FieldCount; i++)
                result[reader.GetName(i)] = reader.GetDataTypeName(i);
            return result;
        }

        private static string? FindCol(IEnumerable<string> cols, params string[] candidates)
        {
            foreach (var c in candidates)
            {
                var found = cols.FirstOrDefault(x => x.Equals(c, StringComparison.OrdinalIgnoreCase));
                if (found != null) return found;
            }
            return null;
        }

        private static void ApplyParams(DbCommand cmd, List<(string name, object value)> ps)
        {
            foreach (var (n, v) in ps) AddParam(cmd, n, v);
        }

        private static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            cmd.Parameters.Add(p);
        }
    }
}
