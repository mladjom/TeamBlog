using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TeamBlog.Data;

namespace TeamBlog.Pages.Articles
{
    public class CategoryNamePageModel : PageModel
    {
        public SelectList CategoryNameSL { get; set; }

        public void PopulateDepartmentsDropDownList(TeamBlogContext _context,
            object selectedCategory = null)
        {
            var categoriesQuery = from c in _context.Category
                                   orderby c.Name // Sort by name.
                                   select c;

            CategoryNameSL = new SelectList(categoriesQuery.AsNoTracking(),
                        "CategoryID", "Name", selectedCategory);
        }
    }
}
