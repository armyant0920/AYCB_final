using System.ComponentModel.DataAnnotations;

namespace CorporateSite.Application.Dtos;

public class NewsEditDto
{
    [Required(ErrorMessage = "標題為必填")]
    [StringLength(200, ErrorMessage = "標題最多 200 字")]
    public string Title { get; set; } = "";

    [StringLength(500, ErrorMessage = "摘要最多 500 字")]
    public string Summary { get; set; } = "";

    public string BodyHtml { get; set; } = "";

    [Required]
    public string Category { get; set; } = "CompanyNews";

    public bool Publish { get; set; }
}
