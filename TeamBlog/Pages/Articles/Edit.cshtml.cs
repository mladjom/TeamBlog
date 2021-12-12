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
        private readonly TeamBlogContext _context;

        public EditModel(
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
            Article? article = await Context.Article.FirstOrDefaultAsync(
                                                         m => m.ID == id);

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
            var article = await _context.Article.AsNoTracking()
                .FirstOrDefaultAsync(m => m.ID == id);

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

            _context.Attach(Article).State = EntityState.Modified;


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
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.ID))
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
            return _context.Article.Any(e => e.ID == id);
        }
    }
}
