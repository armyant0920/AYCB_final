using System.Data;
using CorporateSite.Application.Abstractions.Repositories;
using CorporateSite.Domain.Common;
using CorporateSite.Domain.Entities;
using CorporateSite.Domain.Enums;
using DataProviderInfrastructure;
using Microsoft.Data.SqlClient;

namespace CorporateSite.Infrastructure.Data.Repositories;

public class NewsRepository : INewsRepository
{
    private readonly DataProviderFactory _factory;
    public NewsRepository(DataProviderFactory factory) => _factory = factory;

    private const string SelectColumns = @"
        NewsId, Title, Summary, BodyHtml, Category, CoverImageId,
        Status, PublishedAt, CreatedAt, UpdatedAt, CreatedBy, IsDeleted";

    public News? GetById(int newsId)
    {
        const string sql = $"SELECT {SelectColumns} FROM dbo.News WHERE NewsId = @NewsId AND IsDeleted = 0";
        using var db = _factory.Create();
        return db.Query<News>(sql, [new SqlParameter("@NewsId", SqlDbType.Int) { Value = newsId }])
                 .FirstOrDefault();
    }

    public IReadOnlyList<News> GetPublished(string? category, int top)
    {
        const string sql = $@"
            SELECT TOP (@Top) {SelectColumns}
            FROM dbo.News
            WHERE IsDeleted = 0 AND Status = @Status
              AND (@Category IS NULL OR Category = @Category)
            ORDER BY PublishedAt DESC";

        using var db = _factory.Create();
        return db.Query<News>(sql,
        [
            new SqlParameter("@Top",      SqlDbType.Int)          { Value = top },
            new SqlParameter("@Status",   SqlDbType.Int)          { Value = (int)PublishStatus.Published },
            new SqlParameter("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value }
        ]);
    }

    public PagedResult<News> GetList(int page, int pageSize, string? category = null, int? status = null)
    {
        SqlParameter[] filterPs =
        [
            new("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
        ];

        const string countSql = @"
            SELECT COUNT(*) FROM dbo.News
            WHERE IsDeleted = 0
              AND (@Category IS NULL OR Category = @Category)
              AND (@Status   IS NULL OR Status   = @Status)";

        using var db = _factory.Create();
        int total = db.QueryScalar<int>(countSql, filterPs);

        string listSql = $@"
            SELECT {SelectColumns}
            FROM dbo.News
            WHERE IsDeleted = 0
              AND (@Category IS NULL OR Category = @Category)
              AND (@Status   IS NULL OR Status   = @Status)
            ORDER BY CreatedAt DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        SqlParameter[] listPs =
        [
            new("@Category", SqlDbType.NVarChar, 50) { Value = (object?)category ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
            new("@Offset",   SqlDbType.Int)          { Value = (page - 1) * pageSize },
            new("@PageSize", SqlDbType.Int)          { Value = pageSize },
        ];

        var items = db.Query<News>(listSql, listPs);
        return new PagedResult<News> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public int Insert(News n)
    {
        const string sql = @"
            INSERT INTO dbo.News
                (Title, Summary, BodyHtml, Category, CoverImageId, Status,
                 PublishedAt, CreatedAt, UpdatedAt, CreatedBy, IsDeleted)
            VALUES
                (@Title, @Summary, @BodyHtml, @Category, @CoverImageId, @Status,
                 @PublishedAt, @CreatedAt, @UpdatedAt, @CreatedBy, 0)";

        using var db = _factory.Create();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        return db.ExecuteScalarCommand<int>(new CommandSetting(sql, BuildParams(n), CommandType.Text));
    }

    public int Update(News n)
    {
        const string sql = @"
            UPDATE dbo.News SET
                Title=@Title, Summary=@Summary, BodyHtml=@BodyHtml,
                Category=@Category, CoverImageId=@CoverImageId,
                Status=@Status, PublishedAt=@PublishedAt, UpdatedAt=@UpdatedAt
            WHERE NewsId=@NewsId AND IsDeleted=0";

        var ps = BuildParams(n).Append(new SqlParameter("@NewsId", SqlDbType.Int) { Value = n.NewsId });
        using var db = _factory.Create();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        int rows = db.ExecuteNonQueryCommand(new CommandSetting(sql, ps, CommandType.Text));
        db.GetTransaction().Commit();
        return rows;
    }

    public int SoftDelete(int newsId)
    {
        const string sql = @"
            UPDATE dbo.News SET IsDeleted=1, UpdatedAt=@Now
            WHERE NewsId=@NewsId AND IsDeleted=0";

        SqlParameter[] ps =
        [
            new("@NewsId", SqlDbType.Int)      { Value = newsId },
            new("@Now",    SqlDbType.DateTime2) { Value = DateTime.UtcNow },
        ];
        using var db = _factory.Create();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        int rows = db.ExecuteNonQueryCommand(new CommandSetting(sql, ps, CommandType.Text));
        db.GetTransaction().Commit();
        return rows;
    }

    private static IEnumerable<SqlParameter> BuildParams(News n) =>
    [
        new("@Title",        SqlDbType.NVarChar, 200) { Value = n.Title },
        new("@Summary",      SqlDbType.NVarChar, 500) { Value = n.Summary },
        new("@BodyHtml",     SqlDbType.NVarChar, -1)  { Value = n.BodyHtml },
        new("@Category",     SqlDbType.NVarChar, 50)  { Value = n.Category },
        new("@CoverImageId", SqlDbType.Int)            { Value = n.CoverImageId },
        new("@Status",       SqlDbType.Int)            { Value = (int)n.Status },
        new("@PublishedAt",  SqlDbType.DateTime2)      { Value = (object?)n.PublishedAt ?? DBNull.Value },
        new("@CreatedAt",    SqlDbType.DateTime2)      { Value = n.CreatedAt },
        new("@UpdatedAt",    SqlDbType.DateTime2)      { Value = n.UpdatedAt },
        new("@CreatedBy",    SqlDbType.NVarChar, 100) { Value = n.CreatedBy },
    ];
}
