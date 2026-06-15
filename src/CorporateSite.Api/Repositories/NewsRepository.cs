using System.Data;
using CorporateSite.Api.Models;
using DataProviderInfrastructure;
using Microsoft.Data.SqlClient;

namespace CorporateSite.Api.Repositories;

public class NewsRepository
{
    private readonly string _connStr;
    private readonly DbEngineType _engine;

    public NewsRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");
        _engine = DbEngineType.SqlServer;
    }

    private DataProvideAdapter Db() => new(_connStr, _engine);

    private const string Cols = @"
        NewsId, Title, Summary, BodyHtml, Category, Language,
        Status, PublishedAt, CreatedAt, UpdatedAt, CreatedBy, IsDeleted";

    public List<News> GetPublished(string? category, string? language, int top)
    {
        const string sql = $@"
            SELECT TOP (@Top) {Cols} FROM dbo.News
            WHERE IsDeleted=0 AND Status=1
              AND (@Category IS NULL OR Category=@Category)
              AND (@Language IS NULL OR Language=@Language)
            ORDER BY PublishedAt DESC";

        using var db = Db();
        return db.Query<News>(sql,
        [
            new SqlParameter("@Top",      SqlDbType.Int)          { Value = top },
            new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value },
            new SqlParameter("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
        ]);
    }

    public News? GetById(int id)
    {
        const string sql = $"SELECT {Cols} FROM dbo.News WHERE NewsId=@Id AND IsDeleted=0";
        using var db = Db();
        return db.Query<News>(sql, [new SqlParameter("@Id", SqlDbType.Int) { Value = id }]).FirstOrDefault();
    }

    public PagedResult<News> GetList(int page, int pageSize, string? category, string? language, int? status)
    {
        SqlParameter[] filterPs =
        [
            new("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value },
            new("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
        ];
        const string where = @"WHERE IsDeleted=0
              AND (@Category IS NULL OR Category=@Category)
              AND (@Language IS NULL OR Language=@Language)
              AND (@Status IS NULL OR Status=@Status)";

        using var db = Db();
        int total = db.QueryScalar<int>($"SELECT COUNT(*) FROM dbo.News {where}", filterPs);

        SqlParameter[] listPs =
        [
            new("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value },
            new("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
            new("@Offset",   SqlDbType.Int)          { Value = (page - 1) * pageSize },
            new("@PageSize", SqlDbType.Int)          { Value = pageSize },
        ];
        var items = db.Query<News>(
            $"SELECT {Cols} FROM dbo.News {where} ORDER BY CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
            listPs);

        return new PagedResult<News> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public int Insert(News n)
    {
        const string sql = @"
            INSERT INTO dbo.News
                (Title,Summary,BodyHtml,Category,Language,CoverImageId,Status,PublishedAt,CreatedAt,UpdatedAt,CreatedBy,IsDeleted)
            VALUES
                (@Title,@Summary,@BodyHtml,@Category,@Language,0,@Status,@PublishedAt,@CreatedAt,@UpdatedAt,@CreatedBy,0)";

        using var db = Db();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        int id = db.ExecuteScalarCommand<int>(new CommandSetting(sql, Params(n), CommandType.Text));
        db.GetTransaction().Commit();
        return id;
    }

    public void Update(News n)
    {
        const string sql = @"
            UPDATE dbo.News SET
                Title=@Title, Summary=@Summary, BodyHtml=@BodyHtml,
                Category=@Category, Language=@Language,
                Status=@Status, PublishedAt=@PublishedAt, UpdatedAt=@UpdatedAt
            WHERE NewsId=@NewsId AND IsDeleted=0";

        var ps = Params(n).Append(new SqlParameter("@NewsId", SqlDbType.Int) { Value = n.NewsId });
        using var db = Db();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        db.ExecuteNonQueryCommand(new CommandSetting(sql, ps, CommandType.Text));
        db.GetTransaction().Commit();
    }

    public void SoftDelete(int id)
    {
        const string sql = "UPDATE dbo.News SET IsDeleted=1,UpdatedAt=@Now WHERE NewsId=@Id AND IsDeleted=0";
        using var db = Db();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        db.ExecuteNonQueryCommand(new CommandSetting(sql,
        [
            new SqlParameter("@Id",  SqlDbType.Int)       { Value = id },
            new SqlParameter("@Now", SqlDbType.DateTime2) { Value = DateTime.UtcNow }
        ], CommandType.Text));
        db.GetTransaction().Commit();
    }

    private static IEnumerable<SqlParameter> Params(News n) =>
    [
        new("@Title",       SqlDbType.NVarChar, 200) { Value = n.Title },
        new("@Summary",     SqlDbType.NVarChar, 500) { Value = n.Summary },
        new("@BodyHtml",    SqlDbType.NVarChar, -1)  { Value = n.BodyHtml },
        new("@Category",    SqlDbType.NVarChar, 50)  { Value = n.Category },
        new("@Language",    SqlDbType.NVarChar, 10)  { Value = n.Language },
        new("@Status",      SqlDbType.Int)            { Value = n.Status },
        new("@PublishedAt", SqlDbType.DateTime2)      { Value = (object?)n.PublishedAt ?? DBNull.Value },
        new("@CreatedAt",   SqlDbType.DateTime2)      { Value = n.CreatedAt },
        new("@UpdatedAt",   SqlDbType.DateTime2)      { Value = n.UpdatedAt },
        new("@CreatedBy",   SqlDbType.NVarChar, 100) { Value = n.CreatedBy },
    ];
}
