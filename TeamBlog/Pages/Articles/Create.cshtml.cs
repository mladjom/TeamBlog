using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeamBlog.Authorization;
using TeamBlog.Data;
using TeamBlog.Models;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;


namespace TeamBlog.Pages.Articles
{
    public class CreateModel : BaseArticleModel
    {
        public CreateModel(
            TeamBlogContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
            : base(context, authorizationService, userManager)
        {
        }

        public IActionResult OnGet()
        {
            ViewData["CategoryID"] = new SelectList(Context.Category, "CategoryID", "Name");
            return Page();
        }

        [BindProperty]
        public Article Article { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(List<IFormFile> files)
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

            if (Article.FileForm != null)
            {
                byte[] bytes = null;

                var img = Image.FromStream(Article.FileForm.OpenReadStream());
                var height = img.Height;
                var width = img.Width;
                if (width > 900)
                {
                    var ratio = (double)width / (double)height;
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

            Context.Article.Add(Article);
            await Context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
