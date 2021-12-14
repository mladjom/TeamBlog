using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamBlog.Models
{
    public class Article
    {
        public int ID { get; set; }

        [StringLength(60, MinimumLength = 3)]
        [Required]
        public string Title { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [MinLength(3)]
        [Required]    
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.MultilineText)]
        [MinLength(3)]
        [MaxLength(140)]
        [Required]
        public string Excerpt { get; set; } = string.Empty;


        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Published On")]
        [DataType(DataType.Date)]
        public DateTime PubDate  { get; set; }


        [Range(1, 100)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

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
