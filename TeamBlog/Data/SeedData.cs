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


            context.Category.AddRange(
                new Category
                {
                    Name = "Cat1"
                },
                new Category
                {
                    Name = "Cat2"
                },
                new Category
                {
                    Name = "Cat3"
                }
                );
            context.SaveChanges();


            context.Article.AddRange(
                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et ma",
                    PubDate = DateTime.Now.AddDays(-1),
                    OwnerID = adminID,
                    Status = ArticleStatus.Rejected,
                    CategoryID = 1
                },

                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "But I must explain to you how all this mistaken idea of denouncing pleasure and praising pain was born and I will give you a complete accoun",
                    PubDate = DateTime.Now.AddDays(-2),
                    OwnerID = adminID,
                    Status = ArticleStatus.Submitted,
                    CategoryID = 2
                },

                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "The European languages are members of the same family. Their separate existence is a myth. For science, music, sport, etc, Europe uses the",
                    PubDate = DateTime.Now.AddDays(-3),
                    OwnerID = adminID,
                    Status = ArticleStatus.Approved,
                    CategoryID = 3
                },

                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts. Separated they live in.",
                    PubDate = DateTime.Now.AddDays(-4),
                    OwnerID = adminID,
                    Status = ArticleStatus.Submitted,
                    CategoryID = 1
                },
                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "A wonderful serenity has taken possession of my entire soul, like these sweet mornings of spring which I enjoy with my whole heart. I am alo",
                    PubDate = DateTime.Now.AddDays(-4),
                    OwnerID = adminID,
                    Status = ArticleStatus.Approved,
                    CategoryID = 2
                },
                new Article
                {
                    Title = "Post Title",
                    Content = "<p>Lorem ipsum dolor sit amet. Ab officia molestiae 33 voluptate dolores qui magnam fuga ut eaque mollitia. Non quidem iusto ut delectus deserunt sit vitae molestiae aut repudiandae fugit et eveniet nulla et assumenda voluptates et debitis temporibus. Id accusantium autem aut voluptate nulla id facere tempore ut error nihil eos assumenda sint et odit voluptatem eos dolores fuga. </p><p>Eos repellat autem quo dolorem eligendi et incidunt enim quo deserunt incidunt aut itaque deleniti. Non necessitatibus voluptate qui voluptates quis est enim quaerat vel amet dolorum vel deserunt iure! Eum galisum amet aut maiores nemo et magnam dolore. Aut dolorem ipsum est tenetur quos sit aspernatur eligendi. </p><p>Vel doloremque sint et doloremque placeat aut doloremque pariatur eum omnis unde in fugiat aperiam a officiis corrupti rem placeat officia. Et similique dolores id esse cupiditate ut magni quos. </p><p>Et doloribus voluptas sed quia corrupti est facere numquam est sunt cupiditate ut dolorem tenetur id omnis laudantium qui fuga sequi. Aut quae dolores eos quia dolor ut voluptate repellendus non ipsam dolores. Quo dicta facilis et voluptas praesentium quo magni repudiandae cum tempore eius qui amet rerum. Aut ducimus nihil ea odio doloremque sit adipisci accusamus est dolor alias est unde odit. </p><p>Quo quaerat blanditiis eum galisum soluta aut culpa dolorum ut temporibus iure. Ut animi exercitationem non debitis nobis aut vero iusto sit exercitationem ducimus eos quam inventore ut aliquid quas. Est velit esse aut dolores eius ut consequuntur amet quo asperiores placeat quo nihil libero? Qui explicabo veniam quo praesentium eaque est internos quas sit ipsa magni ut doloribus maxime. </p>",
                    Excerpt = "One morning, when Gregor Samsa woke from troubled dreams, he found himself transformed in his bed into a horrible vermin. He lay on his armo",
                    PubDate = DateTime.Now.AddDays(-6),
                    OwnerID = adminID,
                    Status = ArticleStatus.Approved,
                    CategoryID = 3
                }
            );
            context.SaveChanges();

        }
    }
}
