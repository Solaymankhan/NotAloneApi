namespace NotAlone.Services.Abstract
{
    public interface IFileService
    {
        Task<string?> uploadPostFiles(List<IFormFile> files, string message_post);
        Task<string?> uploadSingleFile(IFormFile singleFile, string blobName);
    }
}
