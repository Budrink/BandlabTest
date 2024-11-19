using Microsoft.EntityFrameworkCore;
using ImageCommentApp.Models;

namespace ImageCommentApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ��������� ������ ����� ����������

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Comments)
                .WithOne()
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // �������������� ���������, ���� �����
        }
    }
}