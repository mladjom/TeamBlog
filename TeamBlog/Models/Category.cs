using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TeamBlog.Models
{
    public class Category
    {
        public int CategoryID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;
        public ICollection<Article> Articles { get; set;}
    }
}
