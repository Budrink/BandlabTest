using ImageCommentApp.Dto;
using ImageCommentApp.Models;

namespace ImageCommentApp.Services
{
    public interface IPostService
    {
        Task<PostDto> CreatePostAsync(string caption, IFormFile image, string userName);
        Task<List<PostDto>> GetPostsAsync();
        Task<List<CommentDto>> GetPostCommentsAsync(Guid postId);
        Task<CommentDto> AddCommentAsync(Guid postId, string content, string userName);
        Task<bool> DeleteCommentAsync(Guid commentId, string userName);
    }

}