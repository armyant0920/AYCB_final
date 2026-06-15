using System.Data;
using CorporateSite.Api.Models;
using DataProviderInfrastructure;
using Microsoft.Data.SqlClient;

namespace CorporateSite.Api.Repositories;

public class ArticleRepository
{
    private readonly string _connStr;
    private readonly DbEngineType _engine;

    public ArticleRepository(IConfiguration config)
    {
        _connStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("ConnectionStrings:Default is not configured.");
        _engine = DbEngineType.SqlServer;
    }

    private DataProvideAdapter Db() => new(_connStr, _engine);

    private const string Cols = @"
        ArticleId, Slug, Title, Summary, BodyHtml, Section, Language,
        SortOrder, Status, PublishedAt, CreatedAt, UpdatedAt, CreatedBy, IsDeleted";

    public List<Article> GetPublished(string? section, string? language)
    {
        const string sql = $@"
            SELECT {Cols} FROM dbo.Article
            WHERE IsDeleted=0 AND Status=1
              AND (@Section  IS NULL OR Section =@Section)
              AND (@Language IS NULL OR Language=@Language)
            ORDER BY SortOrder, PublishedAt DESC";

        using var db = Db();
        return db.Query<Article>(sql,
        [
            new SqlParameter("@Section",  SqlDbType.NVarChar, 50) { Value = (object?)section  ?? DBNull.Value },
            new SqlParameter("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
        ]);
    }

    public Article? GetBySlug(string slug)
    {
        const string sql = $"SELECT {Cols} FROM dbo.Article WHERE Slug=@Slug AND IsDeleted=0 AND Status=1";
        using var db = Db();
        return db.Query<Article>(sql,
            [new SqlParameter("@Slug", SqlDbType.NVarChar, 100) { Value = slug }]).FirstOrDefault();
    }

    public Article? GetById(int id)
    {
        const string sql = $"SELECT {Cols} FROM dbo.Article WHERE ArticleId=@Id AND IsDeleted=0";
        using var db = Db();
        return db.Query<Article>(sql,
            [new SqlParameter("@Id", SqlDbType.Int) { Value = id }]).FirstOrDefault();
    }

    public PagedResult<Article> GetList(int page, int pageSize, string? section, string? language, int? status)
    {
        SqlParameter[] filterPs =
        [
            new("@Section",  SqlDbType.NVarChar, 50) { Value = (object?)section  ?? DBNull.Value },
            new("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
        ];
        const string where = @"WHERE IsDeleted=0
              AND (@Section  IS NULL OR Section =@Section)
              AND (@Language IS NULL OR Language=@Language)
              AND (@Status   IS NULL OR Status  =@Status)";

        using var db = Db();
        int total = db.QueryScalar<int>($"SELECT COUNT(*) FROM dbo.Article {where}", filterPs);

        SqlParameter[] listPs =
        [
            new("@Section",  SqlDbType.NVarChar, 50) { Value = (object?)section  ?? DBNull.Value },
            new("@Language", SqlDbType.NVarChar, 10) { Value = (object?)language ?? DBNull.Value },
            new("@Status",   SqlDbType.Int)          { Value = (object?)status   ?? DBNull.Value },
            new("@Offset",   SqlDbType.Int)          { Value = (page - 1) * pageSize },
            new("@PageSize", SqlDbType.Int)          { Value = pageSize },
        ];
        var items = db.Query<Article>(
            $"SELECT {Cols} FROM dbo.Article {where} ORDER BY SortOrder, CreatedAt DESC OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
            listPs);

        return new PagedResult<Article> { Items = items, TotalCount = total, Page = page, PageSize = pageSize };
    }

    public int Insert(Article a)
    {
        const string sql = @"
            INSERT INTO dbo.Article
                (Slug,Title,Summary,BodyHtml,Section,Language,SortOrder,Status,PublishedAt,CreatedAt,UpdatedAt,CreatedBy,IsDeleted)
            VALUES
                (@Slug,@Title,@Summary,@BodyHtml,@Section,@Language,@SortOrder,@Status,@PublishedAt,@CreatedAt,@UpdatedAt,@CreatedBy,0)";

        using var db = Db();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        int id = db.ExecuteScalarCommand<int>(new CommandSetting(sql, Params(a), CommandType.Text));
        db.GetTransaction().Commit();
        return id;
    }

    public void Update(Article a)
    {
        const string sql = @"
            UPDATE dbo.Article SET
                Slug=@Slug, Title=@Title, Summary=@Summary, BodyHtml=@BodyHtml,
                Section=@Section, Language=@Language, SortOrder=@SortOrder,
                Status=@Status, PublishedAt=@PublishedAt, UpdatedAt=@UpdatedAt
            WHERE ArticleId=@ArticleId AND IsDeleted=0";

        var ps = Params(a).Append(new SqlParameter("@ArticleId", SqlDbType.Int) { Value = a.ArticleId });
        using var db = Db();
        using var conn = db.CreateConnection();
        db.BeginTransaction(conn);
        db.ExecuteNonQueryCommand(new CommandSetting(sql, ps, CommandType.Text));
        db.GetTransaction().Commit();
    }

    public void SoftDelete(int id)
    {
        const string sql = "UPDATE dbo.Article SET IsDeleted=1,UpdatedAt=@Now WHERE ArticleId=@Id AND IsDeleted=0";
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

    private static IEnumerable<SqlParameter> Params(Article a) =>
    [
        new("@Slug",        SqlDbType.NVarChar, 100) { Value = a.Slug },
        new("@Title",       SqlDbType.NVarChar, 200) { Value = a.Title },
        new("@Summary",     SqlDbType.NVarChar, 500) { Value = a.Summary },
        new("@BodyHtml",    SqlDbType.NVarChar, -1)  { Value = a.BodyHtml },
        new("@Section",     SqlDbType.NVarChar, 50)  { Value = a.Section },
        new("@Language",    SqlDbType.NVarChar, 10)  { Value = a.Language },
        new("@SortOrder",   SqlDbType.Int)            { Value = a.SortOrder },
        new("@Status",      SqlDbType.Int)            { Value = a.Status },
        new("@PublishedAt", SqlDbType.DateTime2)      { Value = (object?)a.PublishedAt ?? DBNull.Value },
        new("@CreatedAt",   SqlDbType.DateTime2)      { Value = a.CreatedAt },
        new("@UpdatedAt",   SqlDbType.DateTime2)      { Value = a.UpdatedAt },
        new("@CreatedBy",   SqlDbType.NVarChar, 100) { Value = a.CreatedBy },
    ];
}
