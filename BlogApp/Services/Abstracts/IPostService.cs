using BlogApp.Entity;
using BlogApp.Models;

namespace BlogApp.Services.Abstract
{
    public interface IPostService
    {
        Task<List<Post>> GetPostsAsync(string? tag);
        Task<Post?> GetPostByUrlAsync(string url);
        Task<Comment?> AddCommentAsync(int postId, string text, int userId);
        Task CreatePostAsync(PostCreateViewModel model, int userId);
        Task<List<Post>> GetUserPostsAsync(int userId, string? role);
        Task<Post?> GetPostForEditAsync(int postId);
        Task EditPostAsync(PostCreateViewModel model, int[] tagIds, string? role);
        Task<List<Tag>> GetAllTagsAsync();
    }
}
