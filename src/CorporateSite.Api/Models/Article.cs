namespace CorporateSite.Api.Models;

public class Article
{
    public int ArticleId { get; set; }
    public string Slug { get; set; } = "";        // URL-friendly key, e.g. "about-us"
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Section { get; set; } = "";     // e.g. "About", "ESG", "Investor"
    public string Language { get; set; } = "zh-TW";
    public int SortOrder { get; set; }
    public int Status { get; set; }               // 0=Draft, 1=Published
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public bool IsDeleted { get; set; }
}

public class ArticleEditRequest
{
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Section { get; set; } = "";
    public string Language { get; set; } = "zh-TW";
    public int SortOrder { get; set; }
    public bool Publish { get; set; }
    public string CreatedBy { get; set; } = "";
}
