using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamBlog.Models
{
    public class Category
    {
        public int ID { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public ICollection<Article> Articles { get; set;}
    }
}
