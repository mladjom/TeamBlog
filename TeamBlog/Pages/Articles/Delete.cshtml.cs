using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog.Pages.Articles
{
    public class DeleteModel : BaseArticleModel
    {
        private readonly TeamBlogContext _context;

        public DeleteModel(
            TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        [BindProperty]
        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Article? article = await Context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }
            Article = article;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                     User, Article,
                                                     ArticleOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var article = await Context
                .Article.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ArticleID == id);


            if (article == null)
            {
                return NotFound();
            }

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, article,
                                         ArticleOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Article.Remove(article);
            await Context.SaveChangesAsync();

            //Article = await _context.Article.FindAsync(id);

            //if (Article != null)
            //{
            //    _context.Article.Remove(Article);
            //    await _context.SaveChangesAsync();
            //}

            return RedirectToPage("./Index");
        }
    }
}
