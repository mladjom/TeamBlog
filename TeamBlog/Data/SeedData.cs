using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TeamBlog.Models;
using TeamBlog.Authorization;

namespace TeamBlog.Data
{
    public class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new TeamBlogContext(
                serviceProvider.GetRequiredService<DbContextOptions<TeamBlogContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <Ab123_>

                // The admin user can do anything
                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@site.test");
                await EnsureRole(serviceProvider, adminID, Constants.ArticleAdministratorRole);

                // allowed user can create and edit articles that they create
                var editorID = await EnsureUser(serviceProvider, testUserPw, "editor@site.test");
                await EnsureRole(serviceProvider, editorID, Constants.ArticleEditorRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider, string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider, string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }

        public static void SeedDB(TeamBlogContext context, string adminID)
        {
            if (context == null || context.Article == null)
            {
                throw new ArgumentNullException("Null RazorPagesArticleContext");
            }

            // Look for any Articles.
            if (context.Article.Any())
            {
                return;   // DB has been seeded
            }

            context.Article.AddRange(
                new Article
                {
                    Title = "When Harry Met Sally",
                    ReleaseDate = DateTime.Parse("1989-2-12"),
                    Genre = "Romantic Comedy",
                    Price = 7.99M,
                    Status = ArticleStatus.Approved,
                    OwnerID = adminID
                },

                new Article
                {
                    Title = "Ghostbusters ",
                    ReleaseDate = DateTime.Parse("1984-3-13"),
                    Genre = "Comedy",
                    Price = 8.99M,
                    Status = ArticleStatus.Submitted,
                    OwnerID = adminID
                },

                new Article
                {
                    Title = "Ghostbusters 2",
                    ReleaseDate = DateTime.Parse("1986-2-23"),
                    Genre = "Comedy",
                    Price = 9.99M,
                    Status = ArticleStatus.Rejected,
                    OwnerID = adminID
                },

                new Article
                {
                    Title = "Rio Bravo",
                    ReleaseDate = DateTime.Parse("1959-4-15"),
                    Genre = "Western",
                    Price = 3.99M,
                    Status = ArticleStatus.Approved,
                    OwnerID = adminID
                }
            );
            context.SaveChanges();

        }
    }
}
