using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using BlogApp.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Services.Concrete
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITagRepository _tagRepository;

        public PostService(
            IPostRepository postRepository, 
            ICommentRepository commentRepository, 
            ITagRepository tagRepository)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
        }

        public async Task<List<Post>> GetPostsAsync(string? tag)
        {
            var posts = _postRepository.Posts.Where(i => i.IsActive);

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return await posts.ToListAsync();
        }

        public async Task<Post?> GetPostByUrlAsync(string url)
        {
            return await _postRepository.Posts
                .Include(x => x.User)
                .Include(x => x.Tags)
                .Include(x => x.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(p => p.Url == url);
        }

        public async Task<Comment?> AddCommentAsync(int postId, string text, int userId)
        {
            var post = await _postRepository.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
            if (post == null) return null;

            var entity = new Comment
            {
                PostId = postId,
                Text = text,
                PublishedOn = DateTime.Now,
                UserId = userId
            };

            _commentRepository.CreateComment(entity);
            return entity;
        }

        public async Task CreatePostAsync(PostCreateViewModel model, int userId)
        {
            _postRepository.CreatePost(new Post
            {
                Title = model.Title,
                Content = model.Content,
                Url = model.Url,
                Description = model.Description,
                UserId = userId,
                PublishedOn = DateTime.Now,
                Image = "1.jpg",
                IsActive = false
            });
        }

        public async Task<List<Post>> GetUserPostsAsync(int userId, string? role)
        {
            var posts = _postRepository.Posts;

            if (string.IsNullOrEmpty(role))
            {
                return await posts.Where(i => i.UserId == userId).ToListAsync();
            }

            return await posts.ToListAsync();
        }

        public async Task<Post?> GetPostForEditAsync(int postId)
        {
            return await _postRepository.Posts
                .Include(i => i.Tags)
                .FirstOrDefaultAsync(i => i.PostId == postId);
        }

        public async Task EditPostAsync(PostCreateViewModel model, int[] tagIds, string? role)
        {
            var entityToUpdate = new Post
            {
                PostId = model.PostId,
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                Url = model.Url
            };

            if (role == "admin")
            {
                entityToUpdate.IsActive = model.IsActive;
            }

            _postRepository.EditPost(entityToUpdate, tagIds);
        }

        public async Task<List<Tag>> GetAllTagsAsync()
        {
            return await _tagRepository.Tags.ToListAsync();
        }
    }
}
