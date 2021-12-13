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
    [AllowAnonymous]
    public class DetailsModel : BaseArticleModel
    {
        private readonly TeamBlogContext _context;

        public DetailsModel(
            TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Article? _article = await Context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (_article == null)
            {
                return NotFound();
            }
            Article = _article;

            var isAuthorized = User.IsInRole(Constants.ArticleEditorRole) ||
                               User.IsInRole(Constants.ArticleAdministratorRole);

            var currentUserId = UserManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != Article.OwnerID
                && Article.Status != ArticleStatus.Approved)
            {
                return Forbid();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, ArticleStatus status)
        {
            var article = await Context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (article == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ArticleStatus.Approved)
                                                       ? ArticleOperations.Approve
                                                       : ArticleOperations.Reject;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, article,
                                        contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            article.Status = status;
            Context.Article.Update(article);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
