using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore
{
    public static class SeedData
    {
        public static void FillInTestData(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();

            if (context != null)
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }

                if (!context.Tags.Any())
                {
                    context.Tags.AddRange(
                        new Entity.Tag { Text = "web programming", Url = "web-programlama", Color = TagColors.warning },
                        new Entity.Tag { Text = "backend", Url = "backend", Color = TagColors.info },
                        new Entity.Tag { Text = "frontend", Url = "frontend", Color = TagColors.danger },
                        new Entity.Tag { Text = "fullstack", Url = "fullstack", Color = TagColors.success },
                        new Entity.Tag { Text = "php", Url = "php", Color = TagColors.secondary },
                        new Entity.Tag { Text = "c#", Url = "c#", Color = TagColors.primary }
                    );

                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new Entity.User { UserName = "berkaysimsek", Name="Berkay Şimşek", Email="berkayysimsekk@gmail.com", Password="123456", Image = "p1.jpg" },
                        new Entity.User { UserName = "enginsimsek", Name="Engin Şimşek", Email="enginsimsek@gmail.com", Password="232323", Image = "p2.jpg" }
                    );

                    context.SaveChanges();
                }

                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Entity.Post
                        {
                            Title = "Asp.net core",
                            Content = "Asp.net core dersleri",
                            Url = "aspnet-core",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(6).ToList(),
                            Image = "1.jpg",
                            UserId = 1,
                            Comments = new List<Comment>
                             {
                                new Comment{Text="iyi bir kurs", PublishedOn=DateTime.Now.AddDays(-2), UserId=1},
                                new Comment{Text="çok faydalandığım bir kurs", PublishedOn=DateTime.Now.AddDays(-6), UserId=2},
                             }
                        },
                        new Entity.Post
                        {
                            Title = "Php",
                            Content = "Php dersleri",
                            Url = "php",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(2).ToList(),
                            Image = "2.jpg",
                            UserId = 1
                        },
                        new Entity.Post
                        {
                            Title = "Django",
                            Content = "Django dersleri",
                            Url = "django",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-30),
                            Tags = context.Tags.Take(2).ToList(),
                            Image = "3.jpg",
                            UserId = 2
                        }, new Entity.Post
                        {
                            Title = "React",
                            Content = "React dersleri",
                            Url = "react",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-40),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "3.jpg",
                            UserId = 2
                        }, new Entity.Post
                        {
                            Title = "Angular",
                            Content = "Angular dersleri",
                            Url = "angular",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-50),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "3.jpg",
                            UserId = 2
                        }, new Entity.Post
                        {
                            Title = "Web Tasarım",
                            Content = "Web Tasarım dersleri",
                            Url = "web-tasarim",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-60),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "3.jpg",
                            UserId = 2
                        }
                    );

                    context.SaveChanges();
                }
            }
        }
    }
}