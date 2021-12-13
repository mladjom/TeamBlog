using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog
{
    public class IndexModel : PageModel
    {
        private readonly TeamBlog.Data.TeamBlogContext _context;

        public IndexModel(TeamBlog.Data.TeamBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; }

        public async Task OnGetAsync()
        {
            Article = await _context.Article
                .Include(a => a.Category).ToListAsync();
        }
    }
}
