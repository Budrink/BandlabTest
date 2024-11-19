
namespace ImageCommentApp.Services
{
    public interface IImageService
    {
        Task<(string originalFilePath, string jpgFilePath)> SaveAndProcessImageAsync(Stream imageStream, string originalFileName);
    }

}
