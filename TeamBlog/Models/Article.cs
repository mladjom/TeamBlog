using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace TeamBlog.Models
{
    public class Article
    {
        public int ArticleID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [MinLength(3)]
        [AllowHtml]
        [Required]    
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [MinLength(3)]
        [MaxLength(140)]
        [Required]
        public string Excerpt { get; set; } = string.Empty;

        [Display(Name = "Published On")]
        [DataType(DataType.Date)]
        public DateTime PubDate  { get; set; }

        public string Picture { get; set; }

        public bool IsFeatured { get; set; } = false;

        // user ID from AspNetUser table.
        public string? OwnerID { get; set; }

        public ArticleStatus Status { get; set; }

        public int CategoryID { get; set; }
        public Category Category { get; set; }
    }

    public enum ArticleStatus
    {
        Submitted,
        Approved,
        Rejected
    }


}
