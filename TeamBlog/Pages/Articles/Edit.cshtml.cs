using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Context.Attach(Article).State = EntityState.Modified;

            // Fetch Article from DB to get OwnerID.
            //var article = await Context.Article.AsNoTracking()
            //    .FirstOrDefaultAsync(m => m.ArticleID == id);

            //if (article == null)
            //{
            //    return NotFound();
            //}
            //Article = article;


            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                                         User, Article,
                                         ArticleOperations.Update);

            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

            if (Article.FileForm != null)
            {
                byte[] bytes = null;

                var img = Image.FromStream(Article.FileForm.OpenReadStream());
                var height = img.Height;
                var width = img.Width;
                if (width > 900)
                {
                    var ratio = (double)height / (double)width;
                    var newWidth = 900;
                    var newHeight = (int)(900 * ratio);
                    var resizedImage = new Bitmap(img, newWidth, newHeight);
                    using var imageStream = new MemoryStream();
                    resizedImage.Save(imageStream, ImageFormat.Jpeg);
                    bytes = imageStream.ToArray();
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        Article.FileForm.CopyTo(ms);
                        bytes = ms.ToArray();
                    }
                }

                Article.File = bytes;
                Article.FileName = Article.FileForm.FileName;
            }

            //Article.OwnerID = article.OwnerID;

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
