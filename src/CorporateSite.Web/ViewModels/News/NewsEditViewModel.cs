using System.ComponentModel.DataAnnotations;

namespace CorporateSite.Web.ViewModels.News;

public class NewsEditViewModel
{
    public int? NewsId { get; set; }

    [Required(ErrorMessage = "標題為必填")]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [StringLength(500)]
    public string Summary { get; set; } = "";

    public string BodyHtml { get; set; } = "";

    [Required]
    public string Category { get; set; } = "CompanyNews";

    public bool Publish { get; set; }
}
