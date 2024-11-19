
namespace ImageCommentApp.Models
{
    public class Post
    {
        public Guid Id { get; set; }
        public string Caption { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty; // ������ �� .jpg ����
        public string OriginalImageUrl { get; set; } = string.Empty; // ������ �� ��������
        public string Creator { get; set; } = string.Empty; 
        public DateTime CreatedAt { get; set; }
        public List<Comment> Comments { get; set; }
    }

}
