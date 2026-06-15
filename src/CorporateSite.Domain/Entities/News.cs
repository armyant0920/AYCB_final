using CorporateSite.Domain.Enums;

namespace CorporateSite.Domain.Entities;

public class News
{
    public int NewsId { get; set; }
    public string Title { get; set; } = "";
    public string Summary { get; set; } = "";
    public string BodyHtml { get; set; } = "";
    public string Category { get; set; } = "CompanyNews";
    public int CoverImageId { get; set; }
    public PublishStatus Status { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; } = "";
    public bool IsDeleted { get; set; }
}
