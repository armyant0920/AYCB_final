using System.ComponentModel.DataAnnotations;

namespace CorporateSite.Web.ViewModels.Article;

public class ArticleEditViewModel
{
    public int? ArticleId { get; set; }

    [Required(ErrorMessage = "Slug 為必填（URL 識別鍵）")]
    [RegularExpression(@"^[a-z0-9\-]+$", ErrorMessage = "Slug 只能包含小寫英文、數字與連字號")]
    [StringLength(100)]
    public string Slug { get; set; } = "";

    [Required(ErrorMessage = "標題為必填")]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [StringLength(500)]
    public string Summary { get; set; } = "";

    public string BodyHtml { get; set; } = "";

    [Required]
    public string Section { get; set; } = "About";

    [Required]
    public string Language { get; set; } = "zh-TW";

    public int SortOrder { get; set; }

    public bool Publish { get; set; }
}
