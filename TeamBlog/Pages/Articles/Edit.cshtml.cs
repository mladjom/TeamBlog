using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;

namespace TeamBlog.Pages.Articles
{
    public class EditModel : BaseArticleModel
    {

        public EditModel(
            TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }


        [BindProperty]
        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            Article article = await Context.Article
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }

            Article = article;

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                                      User, Article,
                                                      ArticleOperations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            ViewData["CategoryID"] = new SelectList(Context.Category, "CategoryID", "Name");
            ViewData["OwnerID"] = new SelectList(Context.Users, "Id", "UserName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }


            // Fetch Article from DB to get OwnerID.
            var article = await Context.Article.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ArticleID == id);

            if (article == null)
            {
                return NotFound();
            }
            Article = article;


            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, Article,
                                         ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            Article.OwnerID = article.OwnerID;

            Context.Attach(Article).State = EntityState.Modified;


            if (Article.Status == ArticleStatus.Approved)
            {
                // If the contact is updated after approval, 
                // and the user cannot approve,
                // set the status back to submitted so the update can be
                // checked and approved.
                var canApprove = await AuthorizationService.AuthorizeAsync(User,
                                        Article,
                                        ArticleOperations.Approve);

                if (!canApprove.Succeeded)
                {
                    Article.Status = ArticleStatus.Submitted;
                }
            }

            try
            {
                Context.Update(Article);
                await Context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.ArticleID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return Context.Article.Any(e => e.ArticleID == id);
        }
    }
}
