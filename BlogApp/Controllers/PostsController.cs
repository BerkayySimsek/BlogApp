using System.Security.Claims;
using BlogApp.Models;
using BlogApp.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Controllers
{
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index(string tag)
        {
            var posts = await _postService.GetPostsAsync(tag);
            return View(new PostsViewModel { Posts = posts });
        }

        public async Task<IActionResult> Details(string url)
        {
            var post = await _postService.GetPostByUrlAsync(url);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        [HttpPost]
        public async Task<JsonResult> AddComment(int postId, string text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            if (!int.TryParse(userId, out int parsedUserId))
            {
                return Json(new { success = false, message = "Kullanıcı oturumu bulunamadı." });
            }

            var comment = await _postService.AddCommentAsync(postId, text, parsedUserId);
            if (comment == null)
            {
                return Json(new { success = false, message = "Gönderi bulunamadı." });
            }

            return Json(new
            {
                userName,
                text,
                comment.PublishedOn,
                avatar
            });
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (int.TryParse(userId, out int parsedUserId))
                {
                    await _postService.CreatePostAsync(model, parsedUserId);
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = await _postService.GetUserPostsAsync(userId, role);

            return View(posts);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var post = await _postService.GetPostForEditAsync(id.Value);
            if (post == null) return NotFound();

            ViewBag.Tags = await _postService.GetAllTagsAsync();

            return View(new PostCreateViewModel
            {
                PostId = post.PostId,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                Url = post.Url,
                IsActive = post.IsActive,
                Tags = post.Tags
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(PostCreateViewModel model, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var role = User.FindFirstValue(ClaimTypes.Role);
                await _postService.EditPostAsync(model, tagIds, role);
                return RedirectToAction("List");
            }

            ViewBag.Tags = await _postService.GetAllTagsAsync();
            return View(model);
        }
    }
}
