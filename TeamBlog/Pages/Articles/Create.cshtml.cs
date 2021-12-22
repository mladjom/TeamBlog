using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog.Pages.Articles
{
    public class CreateModel : BaseArticleModel
    {
        //private readonly TeamBlogContext _context;

        public CreateModel(
            TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
           // _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryID"] = new SelectList(Context.Category, "CategoryID", "Name");
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            Article.OwnerID = UserManager.GetUserId(User);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                  User, Article,
                                                  ArticleOperations.Create);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Context.Article.Add(Article);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
