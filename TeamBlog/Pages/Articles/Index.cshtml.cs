using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog.Pages.Articles
{
    [AllowAnonymous]
    public class IndexModel : BaseArticleModel
    {

        public IndexModel(TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IList<Article> Article { get; set; }

        public async Task OnGetAsync()
        {
            var articles = from c in Context.Article
                           select c;


            //ViewData["OwnerID"] = new SelectList(Context.Users, "Id", "UserName");

            //var user = UserManager.Users.FirstOrDefault();





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
                .Include(a => a.Category)
                .ToListAsync();

            //var user = await UserManager.FindByIdAsync(Context.Article.OwnerID);
            var user = UserManager.Users.FirstOrDefault(u => u.Id == currentUserId);
            ViewData["User"] = user;

        }
    }
}
