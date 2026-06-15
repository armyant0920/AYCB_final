namespace CorporateSite.Web.Models;

public class ArticleDto
{
    public int ArticleId { get; set; }
    public string Slug { get; set; } = "";
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Section { get; set; } = "";
    public string Language { get; set; } = "zh-TW";
    public int SortOrder { get; set; }
    public int Status { get; set; }
    public DateTime? PublishedAt { get; set; }
}
