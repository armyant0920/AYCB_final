namespace CorporateSite.Web.Models;

public class NewsDto
{
    public int NewsId { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Category { get; set; } = "";
    public int Status { get; set; }
    public DateTime? PublishedAt { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}
