using System.Security.Claims;
using System.Threading.Tasks;
using BlogApp.Data.Abstract;
using BlogApp.Data.Concrete.EfCore;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Contollers
{
    public class PostsController : Controller
    {

        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;

        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var posts = _postRepository.Posts;

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }
            return View(new PostsViewModel { Posts = await posts.ToListAsync() }
            );
        }

        public async Task<IActionResult> Details(string url)
        {
            return View(await _postRepository
            .Posts
            .Include(x => x.Tags)
            .Include(x => x.Comments)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(p => p.Url == url));
        }

        [HttpPost]
        public JsonResult AddComment(int postId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Json(new { success = false, message = "Kullanıcı oturumu bulunamadı." });
            }

            var post = _postRepository.Posts.FirstOrDefault(p => p.PostId == postId);
            if (post == null)
            {
                return Json(new { success = false, message = "Gönderi bulunamadı." });
            }

            var entity = new Comment
            {
                PostId = postId,
                Text = text,
                PublishedOn = DateTime.Now,
                UserId = parsedUserId
            };

            _commentRepository.CreateComment(entity);

            return Json(new
            {
                userName,
                text,
                entity.PublishedOn,
                avatar
            });
        }
    }
}