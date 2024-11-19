using ImageCommentApp.Data;
using ImageCommentApp.Dto;
using ImageCommentApp.Models;
using ImageCommentApp.Services;
using Microsoft.EntityFrameworkCore;

namespace ImageCommentApp.Services
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IImageService _imageService;

        public PostService(ApplicationDbContext dbContext, IImageService imageService)
        {
            _dbContext = dbContext;
            _imageService = imageService;
        }

        public async Task<PostDto> CreatePostAsync(string caption, IFormFile image, string userName)
        {
            if (image == null || image.Length == 0)
                throw new ArgumentException("No image uploaded.", nameof(image));

            var (originalFilePath, jpgFilePath) = await _imageService.SaveAndProcessImageAsync(image.OpenReadStream(), image.FileName);

            var post = new Post
            {
                Caption = caption,
                ImageUrl = jpgFilePath,
                OriginalImageUrl = originalFilePath,
                Creator = userName ?? "Anonymous",
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Posts.Add(post);
            await _dbContext.SaveChangesAsync();

            return new PostDto
            {
                Id = post.Id,
                Caption = post.Caption,
                ImageUrl = post.ImageUrl,
                CreatedAt = post.CreatedAt,
                LastTwoComments = new List<CommentDto>()
            }; ;
        }

        public async Task<List<PostDto>> GetPostsAsync()
        {
            var posts = await _dbContext.Posts
                .Include(p => p.Comments)
                .OrderByDescending(p => p.Comments.Count) // Сортировка по количеству комментариев в убывающем порядке
                .Select(p => new PostDto
                {
                    Id = p.Id,
                    Caption = p.Caption,
                    ImageUrl = p.ImageUrl,
                    CreatedAt = p.CreatedAt,
                    LastTwoComments = p.Comments
                        .OrderByDescending(c => c.CreatedAt)
                        .Take(2)
                        .Select(c => new CommentDto
                        {
                            Id = c.Id,
                            Content = c.Content,
                            Creator = c.Creator,
                            CreatedAt = c.CreatedAt
                        })
                        .ToList()
                })
                .ToListAsync();

            return posts;
        }

        public async Task<List<CommentDto>> GetPostCommentsAsync(Guid postId)
        {
            var post = await _dbContext.Posts
                .Include(p => p.Comments)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            return post.Comments
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    Creator = c.Creator,
                    CreatedAt = c.CreatedAt
                })
                .ToList();
        }

        public async Task<CommentDto> AddCommentAsync(Guid postId, string content, string creator)
        {
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post == null)
                throw new KeyNotFoundException("Post not found.");

            var comment = new Comment
            {
                Content = content,
                Creator = creator,
                CreatedAt = DateTime.UtcNow,
                PostId = postId
            };

            _dbContext.Comments.Add(comment);
            await _dbContext.SaveChangesAsync();

            return new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                Creator = comment.Creator,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<bool> DeleteCommentAsync(Guid commentId, string currentUser)
        {
            var comment = await _dbContext.Comments.FindAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            if (comment.Creator != currentUser)
                throw new UnauthorizedAccessException("You are not authorized to delete this comment.");

            _dbContext.Comments.Remove(comment);
            await _dbContext.SaveChangesAsync();

            return true;
        }
    }
}
