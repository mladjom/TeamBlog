using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog.Pages.Articles
{
    [AllowAnonymous]
    public class IndexModel : BaseArticleModel
    {
        private readonly TeamBlogContext _context;

        public IndexModel(TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; }

        public async Task OnGetAsync()
        {
            var articles = from c in Context.Article
                           select c;

            var isAuthorized = User.IsInRole(Constants.ArticleEditorRole) ||
                               User.IsInRole(Constants.ArticleAdministratorRole);

            var currentUserId = UserManager.GetUserId(User);

            // Only approved Articles are shown UNLESS you're authorized to see them
            // or you are the owner.
            if (!isAuthorized)
            {
                articles = articles.Where(c => c.Status == ArticleStatus.Approved
                                            || c.OwnerID == currentUserId);
            }

            Article = await articles
                .Include(a => a.Category).ToListAsync();
        }
    }
}
